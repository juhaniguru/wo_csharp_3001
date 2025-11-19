using API.CustomExceptions;
using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class BlogService(DataContext _repository) : IBlogService
    {
        public async Task<Blog> Create(CreateBlogReq requestData, int loggedInUser)
        {

            
            var existingTags = await _repository.Tags.Where(tag => requestData.Tags.Contains(tag.TagText)).ToListAsync();

            var existingTagTexts = existingTags.Select(tag => tag.TagText).ToList();
            var newTagTexts = requestData.Tags.Except(existingTagTexts).ToList();
            var newTagEntities = new List<Tag>();


            // lisätään uudet tagit listaan
            foreach (var text in newTagTexts)
            {
                newTagEntities.Add(new Tag
                {
                    TagText = text
                });

            }

            await _repository.Tags.AddRangeAsync(newTagEntities);
            var allTags = existingTags.Union(newTagEntities).ToList();


            var blogItem = new Blog
            {
                Title = requestData.Title,
                Content = requestData.Content,
                AppUserId = loggedInUser,
                Tags = allTags
            };

            await _repository.Blogs.AddAsync(blogItem);
            await _repository.SaveChangesAsync();
            return blogItem;
        }

        public async Task<IEnumerable<Blog>> GetAll()
        {
            var blogs = await _repository.Blogs.AsNoTracking().ToListAsync();
            return blogs;
        }

        
        public async Task<Blog> GetById(int id)
        {
            // include luo INNER JOININ: SELECT * FROM Blogs INNER JOIN Users ON Usres.Id = Blogs.AppUserId WHERE Blogs.Id = ?
            var blogWithOwner = await _repository.Blogs.Include(b => b.Owner).Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == id);
            if (blogWithOwner == null)
            {
                throw new NotFoundException("blog not found");
            }

            return blogWithOwner;
        }
    }
}