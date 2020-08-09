using AutoMapper;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Infrastructure;
using LuduStack.Domain.Interfaces.Services;

namespace LuduStack.Application.Interfaces
{
    public interface IProfileBaseAppServiceCommon : IBaseAppServiceCommon
    {
        IProfileDomainService ProfileDomainService { get; }
    }
}