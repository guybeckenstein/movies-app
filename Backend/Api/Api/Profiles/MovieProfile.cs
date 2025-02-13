using AutoMapper;
using Api.DTOs;
using Models.Models;

namespace Api.Profiles
{
    class MovieProfile: Profile
    {
        public MovieProfile()
        {
            CreateMap<MovieRequest, MovieResponse>()
                //.ForMember(dest => dest.Genres, opt => opt.MapFrom(src => 
                //    string.IsNullOrEmpty(src.Genre) 
                //        ? new List<string>() 
                //        : src.Genre.Split(new string[] { ", " }, StringSplitOptions.None).ToList()))
                .ForMember(dest => dest.Poster, opt => opt.MapFrom(src => 
                    src.Poster ?? string.Empty));
        }
    }
}
