using AutoMapper;
using BussinessLogic.Contracts.ParserTask;
using DataAccess.Tables.ParserTask;

namespace BussinessLogic.Mappers;

public class AggregateProfile : Profile
{
    public AggregateProfile()
    {
        CreateMap<ParserTask, ParserTaskDto>();
    }
}