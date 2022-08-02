﻿using API.Data;
using API.Data.IRepository;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Models.Response;
namespace API.Services
{
    public class CommentService:ICommentService
    {
        public async Task<Comment?> GetCommentById(SiteDbContext _context, Guid id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }
        public async Task<DbCodes.Codes> CreateComment(SiteDbContext _context, Comment comment)
        {
            try
            {
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch 
            {
                return DbCodes.Codes.Error;
            }
        }
        public async Task<DbCodes.Codes> UpdateComment(SiteDbContext _context, Comment comment)
        {
            try
            {
                _context.Entry(comment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
    }
}