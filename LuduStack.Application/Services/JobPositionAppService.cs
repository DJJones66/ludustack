﻿using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Jobs;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Application.Services
{
    public class JobPositionAppService : ProfileBaseAppService, IJobPositionAppService
    {
        private readonly IJobPositionDomainService jobPositionDomainService;
        private readonly IGamificationDomainService gamificationDomainService;

        public JobPositionAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon
            , IJobPositionDomainService jobPositionDomainService
            , IGamificationDomainService gamificationDomainService) : base(profileBaseAppServiceCommon)
        {
            this.jobPositionDomainService = jobPositionDomainService;
            this.gamificationDomainService = gamificationDomainService;
        }

        #region ICrudAppService

        public OperationResultVo<int> Count(Guid currentUserId)
        {
            try
            {
                int count = jobPositionDomainService.Count();

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public OperationResultListVo<JobPositionViewModel> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<JobPosition> allModels = jobPositionDomainService.GetAll();

                IEnumerable<JobPositionViewModel> vms = mapper.Map<IEnumerable<JobPosition>, IEnumerable<JobPositionViewModel>>(allModels);

                return new OperationResultListVo<JobPositionViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<JobPositionViewModel>(ex.Message);
            }
        }

        public OperationResultVo GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = jobPositionDomainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo<JobPositionViewModel> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                JobPosition model = jobPositionDomainService.GetById(id);

                if (model == null)
                {
                    return new OperationResultVo<JobPositionViewModel>("JobPosition not found!");
                }

                JobPositionViewModel vm = mapper.Map<JobPositionViewModel>(model);

                foreach (JobApplicantViewModel applicant in vm.Applicants)
                {
                    UserProfile profile = GetCachedProfileByUserId(applicant.UserId);
                    if (profile != null)
                    {
                        applicant.JobPositionId = id;
                        applicant.Name = profile.Name;
                        applicant.Location = profile.Location;
                        applicant.ProfileImageUrl = UrlFormatter.ProfileImage(applicant.UserId, 84);
                        applicant.CoverImageUrl = UrlFormatter.ProfileCoverImage(applicant.UserId, profile.Id, null, profile.HasCoverImage, 300);
                    }
                }

                vm.Applicants = vm.Applicants.OrderByDescending(x => x.Score).ToList();

                vm.CurrentUserApplied = model.Applicants.Any(x => x.UserId == currentUserId);
                vm.ApplicantCount = model.Applicants.Count;

                if (vm.Benefits == null)
                {
                    vm.Benefits = new List<JobPositionBenefitVo>();
                }
                List<JobPositionBenefit> allBenefits = Enum.GetValues(typeof(JobPositionBenefit)).Cast<JobPositionBenefit>().Where(x => x != JobPositionBenefit.NotInformed).ToList();

                foreach (JobPositionBenefit benefit in allBenefits)
                {
                    if (!vm.Benefits.Any(x => x.Benefit == benefit))
                    {
                        vm.Benefits.Add(new JobPositionBenefitVo { Benefit = benefit, Available = false });
                    }
                }

                SetPermissions(currentUserId, vm);

                if (string.IsNullOrWhiteSpace(vm.Reference))
                {
                    vm.Reference = vm.Id.ToString();
                }

                return new OperationResultVo<JobPositionViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<JobPositionViewModel>(ex.Message);
            }
        }

        public OperationResultVo Remove(Guid currentUserId, Guid id)
        {
            try
            {
                // validate before

                jobPositionDomainService.Remove(id);

                unitOfWork.Commit();

                return new OperationResultVo(true, "That Job Position is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo<Guid> Save(Guid currentUserId, JobPositionViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                JobPosition model;

                JobPosition existing = jobPositionDomainService.GetById(viewModel.Id);
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<JobPosition>(viewModel);
                }

                if (viewModel.Id == Guid.Empty)
                {
                    jobPositionDomainService.Add(model);
                    viewModel.Id = model.Id;

                    pointsEarned += gamificationDomainService.ProcessAction(currentUserId, PlatformAction.JobPositionPost);
                }
                else
                {
                    jobPositionDomainService.Update(model);
                }

                unitOfWork.Commit();

                viewModel.Id = model.Id;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        #endregion ICrudAppService

        public OperationResultVo GenerateNew(Guid currentUserId, JobPositionOrigin origin)
        {
            try
            {
                JobPosition newJobPosition = jobPositionDomainService.GenerateNewJobPosition(currentUserId, origin);

                JobPositionViewModel newVm = mapper.Map<JobPositionViewModel>(newJobPosition);

                return new OperationResultVo<JobPositionViewModel>(newVm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetAllAvailable(Guid currentUserId)
        {
            try
            {
                IEnumerable<JobPosition> allModels = jobPositionDomainService.GetAllAvailable();

                List<JobPositionViewModel> vms = mapper.Map<IEnumerable<JobPosition>, IEnumerable<JobPositionViewModel>>(allModels).ToList();

                foreach (JobPositionViewModel vm in vms)
                {
                    SetPermissions(currentUserId, vm);
                    vm.ApplicantCount = vm.Applicants.Count;
                    vm.CurrentUserApplied = vm.Applicants.Any(x => x.UserId == currentUserId);
                }

                vms = vms.OrderByDescending(x => x.CreateDate).ToList();

                return new OperationResultListVo<JobPositionViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetAllMine(Guid currentUserId)
        {
            try
            {
                IEnumerable<JobPosition> allModels = jobPositionDomainService.GetByUserId(currentUserId);

                List<JobPositionViewModel> vms = mapper.Map<IEnumerable<JobPosition>, IEnumerable<JobPositionViewModel>>(allModels).ToList();

                foreach (JobPositionViewModel vm in vms)
                {
                    SetPermissions(currentUserId, vm);
                    vm.ApplicantCount = vm.Applicants.Count;
                }

                vms = vms.OrderByDescending(x => x.CreateDate).ToList();

                return new OperationResultListVo<JobPositionViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetMyPositionsStats(Guid currentUserId)
        {
            try
            {
                Dictionary<JobPositionStatus, int> stats = jobPositionDomainService.GetPositionsStats(currentUserId);

                Dictionary<string, int> model = new Dictionary<string, int>();

                List<JobPositionStatus> allStatus = Enum.GetValues(typeof(JobPositionStatus)).Cast<JobPositionStatus>().ToList();

                foreach (JobPositionStatus status in allStatus)
                {
                    if (stats.ContainsKey(status))
                    {
                        int statusStats = stats[status];
                        model.Add(status.ToDisplayName(), statusStats);
                    }
                    else
                    {
                        model.Add(status.ToDisplayName(), 0);
                    }
                }

                return new OperationResultVo<Dictionary<string, int>>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo GetMyApplications(Guid currentUserId)
        {
            try
            {
                List<JobPositionApplicationVo> positionIds = jobPositionDomainService.GetApplicationsByUserId(currentUserId);

                return new OperationResultListVo<JobPositionApplicationVo>(positionIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo Apply(Guid currentUserId, Guid jobPositionId, string email, string coverLetter)
        {
            try
            {
                int pointsEarned = 0;

                JobPosition jobPosition = jobPositionDomainService.GetById(jobPositionId);

                if (jobPosition == null)
                {
                    return new OperationResultVo("Unable to identify the job position you are applying for.");
                }

                bool alreadyApplyed = jobPosition.Applicants.Any(x => x.UserId == currentUserId);
                if (alreadyApplyed)
                {
                    return new OperationResultVo("You already applyed for this job position.");
                }

                jobPositionDomainService.AddApplicant(currentUserId, jobPositionId, email, coverLetter);

                unitOfWork.Commit();

                return new OperationResultVo<Guid>(jobPosition.UserId, pointsEarned, "You have applyed to this Job Position!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo ChangeStatus(Guid currentUserId, Guid jobPositionId, JobPositionStatus selectedStatus)
        {
            try
            {
                JobPosition jobPosition = jobPositionDomainService.GetById(jobPositionId);

                if (jobPosition == null)
                {
                    return new OperationResultVo("Idea not found!");
                }

                jobPosition.Status = selectedStatus;

                jobPositionDomainService.Update(jobPosition);

                unitOfWork.Commit();

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public OperationResultVo RateApplicant(Guid currentUserId, Guid jobPositionId, Guid userId, decimal score)
        {
            try
            {
                JobPosition jobPosition = jobPositionDomainService.GetById(jobPositionId);

                if (jobPosition == null)
                {
                    return new OperationResultVo("Idea not found!");
                }

                foreach (JobApplicant applicant in jobPosition.Applicants)
                {
                    if (applicant.UserId == userId)
                    {
                        applicant.Score = score;
                    }
                }

                jobPositionDomainService.Update(jobPosition);

                unitOfWork.Commit();

                return new OperationResultVo(true, "Applicant rated!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private static void SetPermissions(Guid currentUserId, JobPositionViewModel vm)
        {
            SetBasePermissions(currentUserId, vm);

            vm.Permissions.CanConnect = !string.IsNullOrWhiteSpace(vm.Url) || (vm.Status == JobPositionStatus.OpenForApplication && (!vm.ClosingDate.HasValue || DateTime.Today <= vm.ClosingDate.Value.Date));
        }
    }
}