using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.Dtos.Community; // Namespace dos DTOs de Community
using MyBeast.API.DTOs.User.Output; // Namespace do UserSummaryDto
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MyBeast.API.DTOs.Community.Output; // Adicionado para Select e Count

namespace MyBeast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityPostsController : ControllerBase
    {
        private readonly ICommunityPostService _postService;

        public CommunityPostsController(ICommunityPostService postService)
        {
            _postService = postService;
        }

        // GET /api/CommunityPosts?pageNumber=1&pageSize=10 - Retorna Lista de PostDto
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDto>))] // Atualizado
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // O serviço/repositório já deve incluir User e PostReactions
            var posts = await _postService.GetCommunityFeedAsync(pageNumber, pageSize);

            // Mapear Model para Dto
            var postDtos = posts.Select(MapToDto); // Usa método auxiliar

            return Ok(postDtos);
        }

        // GET /api/CommunityPosts/{id} - Retorna PostDto
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostById(int id)
        {
            // O serviço/repositório já deve incluir User e PostReactions
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Mapear Model para Dto
            var postDto = MapToDto(post);
            return Ok(postDto);
        }

        // POST /api/CommunityPosts - Retorna PostDto
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto postDto) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = postDto.UserId;

            try
            {
                var postToCreate = new CommunityPost
                {
                    UserId = requestingUserId, // Usar ID autenticado
                    Title = postDto.Title,
                    Content = postDto.Content ?? string.Empty,
                    Type = postDto.Type
                };
                var newPost = await _postService.CreatePostAsync(postToCreate);

                // Re-buscar para garantir includes (User) para o DTO de resposta
                var createdPostWithDetails = await _postService.GetPostByIdAsync(newPost.PostId);
                if (createdPostWithDetails == null) return BadRequest("Erro ao buscar post criado.");

                // Mapear para DTO de resposta
                var newPostDto = MapToDto(createdPostWithDetails);

                return CreatedAtAction(nameof(GetPostById), new { id = newPostDto.PostId }, newPostDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/CommunityPosts/{id} - Retorna PostDto
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDto))] // Atualizado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> UpdatePost(int id, [FromBody] UpdatePostDto updateDto, [FromQuery] int userId) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                // O serviço só precisa dos dados a serem atualizados
                var updatedPost = await _postService.UpdatePostAsync(id, updateDto.Title ?? "", updateDto.Content ?? "");

                // Re-buscar para garantir includes (User, Reactions) para o DTO de resposta
                var updatedPostWithDetails = await _postService.GetPostByIdAsync(id);
                if (updatedPostWithDetails == null) return NotFound(); // Segurança

                // Mapear para DTO de resposta
                var updatedPostDto = MapToDto(updatedPostWithDetails);
                return Ok(updatedPostDto);
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // DELETE /api/CommunityPosts/{id} - Sem mudança, retorna NoContent
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] int userId) // Temporário userId
        {
            // TODO: Obter userId autenticado
            int requestingUserId = userId;

            try
            {
                await _postService.DeletePostAsync(id, requestingUserId);
                return NoContent();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) when (ex.Message.Contains("permissão")) { return Forbid(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // POST /api/CommunityPosts/{id}/react - Sem mudança, retorna Ok simples
        [HttpPost("{id}/react")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactToPost(int id, [FromBody] ReactToPostDto reactionDto) // DTO de Entrada
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // TODO: Obter userId autenticado
            int requestingUserId = reactionDto.UserId;

            try
            {
                await _postService.ReactToPostAsync(id, requestingUserId, reactionDto.ReactionType);
                return Ok();
            }
            // ... (catch blocks como antes) ...
            catch (Exception ex) when (ex.Message.Contains("não encontrado")) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private PostDto MapToDto(CommunityPost post)
        {
            if (post == null) return null; // Segurança

            return new PostDto
            {
                PostId = post.PostId,
                AuthorInfo = post.User == null ? null : new UserSummaryDto // Mapeia o User incluído
                {
                    UserId = post.User.UserId,
                    Username = post.User.Username
                },
                Title = post.Title,
                Content = post.Content,
                Type = post.Type,
                CreatedAt = post.CreatedAt,
                // Calcula contagem de reações (requer Include(p => p.PostReactions))
                Upvotes = post.PostReactions?.Count(r => r.ReactionType == "Upvote") ?? 0,
                Downvotes = post.PostReactions?.Count(r => r.ReactionType == "Downvote") ?? 0
            };
        }
    }
}