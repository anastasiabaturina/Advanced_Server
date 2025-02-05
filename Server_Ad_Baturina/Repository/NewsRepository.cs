using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models;

namespace Server_Ad_Baturina.Repository;

public class NewsRepository : INewsRepository
{
    private readonly Context _context;
    private readonly IMapper _mapper;

    public NewsRepository(Context context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateAsync(
        NewsEntity newsEntity, 
        CancellationToken cancellationToken)
    {
        newsEntity.Tags = await GetTagsAsync(newsEntity.Tags, cancellationToken);
        _context.News.Add(newsEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<TagsEntity>> GetTagsAsync(
        List<TagsEntity> tagsEntities,
        CancellationToken cancellationToken)
    {
        var respnoseTags = new List<TagsEntity>();

        var saveTags = new List<TagsEntity>();
        
        foreach (var item in tagsEntities)
        {
            var existingTag = await _context.Tags.FirstOrDefaultAsync(x => x.Title == item.Title, cancellationToken);
        
            if (existingTag == null)
            {
                var newTag = new TagsEntity{ Title = item.Title};
                saveTags.Add(newTag);
            }
            else if (existingTag.Title == item.Title)
            {
                respnoseTags.Add(existingTag);
            }
        }

        tagsEntities = respnoseTags;
        _context.Tags.AddRange(saveTags);
        respnoseTags.AddRange(saveTags);
        return tagsEntities;
    }

    public async Task PutAsync(
        Guid id, 
        string description,
        string image, 
        string[] tags, 
        string title, 
        CancellationToken cancellationToken)
    {
        var newTags = _mapper.Map<List<TagsEntity>>(tags); 

        var news = await _context.News.FirstOrDefaultAsync(x => x.Id == id);

        news.Description = description;
        news.Tags = await GetTagsAsync(newTags, cancellationToken);
        news.Title = title;
        news.Image = image;

        await _context.SaveChangesAsync();
    }

    public async Task<bool> FindAsync(Guid id, CancellationToken cancellationToken) => await _context.News.AnyAsync(x => x.Id == id, cancellationToken);

    public async Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var delete = await _context.News.FindAsync(id,cancellationToken);
        _context.News.Remove(delete);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<NewsEntity>> GetPaginatedAsync(
        int page, 
        int perPage, 
        CancellationToken cancellationToken) => await _context.News
            .Include(x => x.User)
            .Include(x => x.Tags)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);

    public async Task<long> GetAllAsync(CancellationToken cancellationToken) => await _context.News.CountAsync(cancellationToken);
    
    public async Task<NewsAndCount> FilterAsync(
        string author, 
        string keywords, 
        string[] tags, 
        int page, 
        int perPage,
        CancellationToken cancellationToken)
    {
        var query = _context.News
         .Include(x => x.User)
         .Include(x => x.Tags)
         .AsQueryable();

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(x => x.User.Name == author);
        }

        if (!string.IsNullOrEmpty(keywords))
        {
            query = query.Where(x => x.Description.Contains(keywords) || x.Title.Contains(keywords));
        }

        if (tags != null && tags.Length > 0)
        {
            query = query.Where(x => x.Tags.Any(t => tags.Contains(t.Title)));
        }

        var count = await query.CountAsync(cancellationToken);

        var news = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);

        return new NewsAndCount
        {
            News = news,
            Count = count
        };
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken) => await _context.News.CountAsync(cancellationToken);
    
    public async Task<NewsAndCount> GetUserNewsAsync(
        int page, 
        int perPage, 
        Guid id, 
        CancellationToken cancellationToken)
    {
        var query =  _context.News
            .Include(x => x.User)
            .Where(u => u.User.Id == id)
            .Include(x => x.Tags)
            .AsQueryable();

        var count = await query.CountAsync(cancellationToken);

        var news = await query
            .Skip((page - 1) * perPage)
            .Take(perPage).ToListAsync(cancellationToken);

        return new NewsAndCount
        {
            News = news,
            Count = count
        };
    }
}