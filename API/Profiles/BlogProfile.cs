using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Profiles
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<Blog, BlogDto>();
        }
    }
}