using AutoMapper;
using Movies.Models.DTOs;
using Movies.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Movies.Services.Profiles;

public sealed class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<MovieRequest, MovieResponse>()
            .ForMember(dest => 
                dest.Genres, 
                opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Genre) ? new List<string>() : src.Genre.Split(new string[] { ", " }, StringSplitOptions.None).ToList()))
            .ForMember(dest => 
                dest.PosterImg, 
                opt => opt.MapFrom(src => src.Poster ?? string.Empty));
    }
}
