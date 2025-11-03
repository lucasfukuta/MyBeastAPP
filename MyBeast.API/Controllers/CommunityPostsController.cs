using Microsoft.AspNetCore.Mvc;
using MyBeast.Application.Interfaces;
using MyBeast.Domain.Models;
using MyBeast.API.DTOs.Community.Input;  // DTOs de Entrada
using MyBeast.API.DTOs.Community.Output; // DTOs de Saída
using MyBeast.API.DTOs.User.Output;    // Para UserSummaryDto
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Para Select
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using System.Security.Claims; // Para Claims
using System.IdentityModel.Tokens.Jwt;
using MyBeast.API.DTOs.Community; // Para JwtRegisteredClaimNames

namespace MyBeast.API.Controllers
{
    [Authorize] // REQUER AUTENTICAÇÃO PARA TODOS OS ENDPOINTS
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityPostsController : ControllerBase
    {
        private readonly ICommunityPostService _postService;

        public CommunityPostsController(ICommunityPostService postService)
        {
            _postService = postService;
        }

        // GET /api/CommunityPosts?pageNumber=1&pageSize=10 (Feed Paginado)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDto>))]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var posts = await _postService.GetCommunityFeedAsync(pageNumber, pageSize);
            var postDtos = posts.Select(MapToDto);
            return Ok(postDtos);
        }

        // GET /api/CommunityPosts/me (Busca posts do usuário logado)
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDto>))]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetMyPosts()
        {
            var userId = GetAuthenticatedUserId(); // Obtém ID do token
            var posts = await _postService.GetPostsByUserIdAsync(userId);
            var postDtos = posts.Select(MapToDto);
            return Ok(postDtos);
        }

        // GET /api/CommunityPosts/user/{userId} (Busca posts de um usuário específico)
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostDto>))]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPostsByUser(int userId)
        {
            // Qualquer usuário logado pode ver os posts de outro
            var posts = await _postService.GetPostsByUserIdAsync(userId);
            var postDtos = posts.Select(MapToDto);
            return Ok(postDtos);
        }

        // GET /api/CommunityPosts/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) return NotFound(); // Verificação manual de nulo mantida
            var postDto = MapToDto(post);
            return Ok(postDto);
        }

        // POST /api/CommunityPosts (Cria um post para o usuário logado)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // User não encontrado
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto postDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            var postToCreate = new CommunityPost
            {
                Title = postDto.Title,
                Content = postDto.Content ?? string.Empty,
                Type = postDto.Type
            };
            var newPost = await _postService.CreatePostAsync(postToCreate, requestingUserId); // Passa o ID do token

            var createdPostWithDetails = await _postService.GetPostByIdAsync(newPost.PostId);
            if (createdPostWithDetails == null) return BadRequest("Erro ao buscar post criado.");

            var newPostDto = MapToDto(createdPostWithDetails);
            return CreatedAtAction(nameof(GetPostById), new { id = newPostDto.PostId }, newPostDto);
        }

        // PUT /api/CommunityPosts/{id} (Edita um post do usuário logado)
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDto>> UpdatePost(int id, [FromBody] UpdatePostDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            var updatedPost = await _postService.UpdatePostAsync(id, updateDto.Title ?? "", updateDto.Content ?? "", requestingUserId); // Passa o ID do token

            var updatedPostWithDetails = await _postService.GetPostByIdAsync(id);
            if (updatedPostWithDetails == null) return NotFound();

            var updatedPostDto = MapToDto(updatedPostWithDetails);
            return Ok(updatedPostDto);
        }

        // DELETE /api/CommunityPosts/{id} (Deleta um post do usuário logado)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(int id)
        {
            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            await _postService.DeletePostAsync(id, requestingUserId); // Passa o ID do token
            return NoContent();
        }

        // POST /api/CommunityPosts/{id}/react (Reage a um post como o usuário logado)
        [HttpPost("{id}/react")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactToPost(int id, [FromBody] ReactToPostDto reactionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var requestingUserId = GetAuthenticatedUserId(); // Obtém ID do token

            // Bloco try-catch REMOVIDO
            await _postService.ReactToPostAsync(id, requestingUserId, reactionDto.ReactionType); // Passa o ID do token
            return Ok();
        }

        // --- MÉTODO AUXILIAR DE MAPEAMENTO ---
        private PostDto MapToDto(CommunityPost post)
        {
            if (post == null) return null;
            return new PostDto
            {
                PostId = post.PostId,
                AuthorInfo = post.User == null ? null : new UserSummaryDto
                {
                    UserId = post.User.UserId,
                    Username = post.User.Username
                },
                Title = post.Title,
                Content = post.Content,
                Type = post.Type,
                CreatedAt = post.CreatedAt,
                Upvotes = post.PostReactions?.Count(r => r.ReactionType == "Upvote") ?? 0,
                Downvotes = post.PostReactions?.Count(r => r.ReactionType == "Downvote") ?? 0
            };
        }

        // --- MÉTODO AUXILIAR DE AUTORIZAÇÃO ---
        private int GetAuthenticatedUserId()
        {
            var userIdString = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new Exception("ID de usuário não encontrado no token.");
            }
            return userId;
        }
    }
}