using BCBlog.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace BCBlog.Helpers.Extensions
{
    public static class PagedListExtensions
    {

        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IOrderedQueryable<T> query, int page, int pageSize) where T : class
        {
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            List<T> data = [];

            try
            {
                data = await query.Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync();
            }
            catch (InvalidOperationException)
            {
                data = [.. query.Skip(pageSize * (page - 1)).Take(pageSize)];
            }

            return new PagedList<T>
            {
                Page = page,
                TotalPages = totalPages,    
                TotalItems = totalItems,
                Data = data,
            };
        }

    }
}
