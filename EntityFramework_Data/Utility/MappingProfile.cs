using AutoMapper;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<CreateOrUpdateTaskDto, TaskItem>();
        }
    }
}
