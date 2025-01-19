using System;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike userLike)
    {
        context.Likes.Add(userLike);
    }
    public void DeleteLike(UserLike userLike)
    {
        context.Likes.Remove(userLike);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.LikedUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int likedUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<PagedList<MemberDTO>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDTO> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(like => like.SourceUserId == likesParams.UserId)
                    .Select(like => like.LikedUser)
                    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes
                    .Where(like => like.LikedUserId == likesParams.UserId)
                    .Select(like => like.SourceUser)
                    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                .Where(like => like.SourceUserId == likesParams.UserId && likeIds.Contains(like.LikedUserId))
                .Select(like => like.SourceUserId)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<MemberDTO>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
