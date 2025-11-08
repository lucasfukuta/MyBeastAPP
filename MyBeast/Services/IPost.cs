using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBeast.Services
{
    //interface IPost com métodos para criar, ler, atualizar e deletar posts
    internal interface IPost
    {
        Task<bool> CreatePostAsync(string url, Post post);
        Task<Post> GetPostAsync(string url);
        Task<IEnumerable<Post>> GetPostsAsync(string url);
        Task<bool> UpdatePostAsync(string url, Post post);
        Task<bool> DeletePostAsync(string url);
    }
}
