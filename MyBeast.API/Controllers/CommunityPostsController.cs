using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.Community; // Importa DTOs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota: /api/CommunityPosts
    public class CommunityPostsController : ControllerBase
    {
        private readonly ICommunityPostService _postService;

        public CommunityPostsController(ICommunityPostService postService)
        {
            _postService = postService;
        }

        // GET /api/CommunityPosts?pageNumber=1&pageSize=10 (Feed Paginado)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CommunityPost>))]
        public async Task<ActionResult<IEnumerable<CommunityPost>>> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var posts = await _postService.GetCommunityFeedAsync(pageNumber, pageSize);
            return Ok(posts);
        }

        // GET /api/CommunityPosts/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityPost))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityPost>> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            // TODO: Mapear para um PostDto que inclua contagem de reações, se necessário
            return Ok(post);
        }

        // POST /api/CommunityPosts
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommunityPost))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CommunityPost>> CreatePost([FromBody] CreatePostDto postDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var postToCreate = new CommunityPost
                {
                    UserId = postDto.UserId,
                    Title = postDto.Title,
                    Content = postDto.Content ?? string.Empty, // Garante não nulo
                    Type = postDto.Type
                    // CreatedAt é definido pelo serviço
                };
                var newPost = await _postService.CreatePostAsync(postToCreate);
                return CreatedAtAction(nameof(GetPostById), new { id = newPost.PostId }, newPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/CommunityPosts/{id} (Editar post)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityPost))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommunityPost>> UpdatePost(int id, [FromBody] UpdatePostDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // TODO: Obter userId do usuário logado (autenticação) em vez de confiar no DTO
                var updatedPost = await _postService.UpdatePostAsync(id, updateDto.Title ?? "", updateDto.Content ?? ""); // Passa strings vazias se nulo
                return Ok(updatedPost);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão"))
            {
                return Forbid(ex.Message); // Ou Unauthorized
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/CommunityPosts/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para erro de permissão
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] int userId) // Temporário: pegando userId da query
        {
            // TODO: Obter userId do usuário logado (autenticação)
            if (userId <= 0) return BadRequest("UserId inválido.");

            try
            {
                await _postService.DeletePostAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex) when (ex.Message.Contains("permissão"))
            {
                return Forbid(ex.Message); // Ou Unauthorized
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/CommunityPosts/{id}/react (Reagir a um post)
        [HttpPost("{id}/react")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactToPost(int id, [FromBody] ReactToPostDto reactionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // TODO: Obter userId do usuário logado (autenticação) em vez de confiar no DTO
                await _postService.ReactToPostAsync(id, reactionDto.UserId, reactionDto.ReactionType);
                return Ok(); // Retorna 200 OK simples
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message); // Post ou Usuário não encontrado
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Ex: Tipo inválido
            }
        }
    }
}