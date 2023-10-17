using StartupCentral.Code.Class;
using StartupCentral.Code.Model;
using StartupCentral.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;



namespace StartupCentral.Code.ApiControllers
{
    public class ReportsApiController : UmbracoApiController
    {
        private UserMessageService messageService;
        private readonly IUnitOfWork _unitOfWork;
       // private const string allowedMembers = "31957, 26886, 26502";

        public ReportsApiController()
        {
            messageService = new UserMessageService();
            _unitOfWork = new UnitOfWork();
        }

        [HttpGet]
        // [Umbraco.Web.WebApi.UmbracoAuthorize]
       // [Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<MemberData> MembersReport()
        {

            var allMembers = Services.MemberService.GetAllMembers();
            //var sorted = allMembers.OrderBy(x => x.Id);
            var membersList = new List<MemberData>();
            foreach (var member in allMembers)
            {
                if (member != null)
                {
                    membersList.Add(new MemberData
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Email = member.Email,
                        CreateDate = member.CreateDate,
                        ContentType = member.ContentType.Alias,
                        IsApproved = member.IsApproved,
                        CVR = member.GetValue<string>("cVR") ?? "-",
                        City = member.GetValue<string>("wwcity") ?? "-",
                        Address = member.GetValue<string>("wwaddress") ?? "-",
                        PostCode = member.GetValue<string>("wwpostnummer") ?? "-",
                        Mobile = member.GetValue<string>("wwmobile") ?? "-"
                    });
                }
            }
            return membersList;
        }
        [HttpGet]
        public List<MemberData> StudentsReport()
        {

            var allStudents = Services.MemberService.GetMembersByMemberType("student");
            //var sorted = allMembers.OrderBy(x => x.Id);
            var studentsList = new List<MemberData>();
            foreach (var member in allStudents)
            {
                if (member != null)
                {
                    studentsList.Add(new MemberData
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Email = member.Email,
                        CreateDate = member.CreateDate,
                        ContentType = member.ContentType.Alias,
                        IsApproved = member.IsApproved,
                        CVR = member.GetValue<string>("cVR") ?? "-",
                        City = member.GetValue<string>("wwcity") ?? "-",
                        Address = member.GetValue<string>("wwaddress") ?? "-",
                        PostCode = member.GetValue<string>("wwpostnummer") ?? "-",
                        Mobile = member.GetValue<string>("wwmobile") ?? "-"
                    });
                }
            }
            return studentsList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<MemberData> CoachesReport()
        {
            var allCoaches = Services.MemberService.GetMembersByMemberType("coach1");
            var coachDataList = new List<MemberData>();
            foreach (var coach in allCoaches)
            {
                if (coach != null)
                {
                    coachDataList.Add(new MemberData
                    {
                        Id = coach.Id,
                        Name = coach.Name,
                        Email = coach.Email,
                        CreateDate = coach.CreateDate,
                        ContentType = coach.ContentType.Alias,
                        IsApproved = coach.IsApproved,
                        IsAvailable = !Convert.ToBoolean(coach.GetValue("wwunavailable"))
                    });
                }
            }
            return coachDataList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<MemberData> PartnersReport()
        {
            var allpartners = Services.MemberService.GetMembersByMemberType("partner");
            var partnerDataList = new List<MemberData>();
            foreach (var partner in allpartners)
            {
                if (partner != null)
                {
                    partnerDataList.Add(new MemberData
                    {
                        Id = partner.Id,
                        Name = partner.Name,
                        Email = partner.Email,
                        CreateDate = partner.CreateDate,
                        ContentType = partner.ContentType.Alias,
                        IsApproved = partner.IsApproved,
                        IsAvailable = !Convert.ToBoolean(partner.GetValue("wwunavailable")),
                        IsBinded = Convert.ToBoolean(partner.GetValue("binding"))
                    });
                }
            }
            return partnerDataList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public async Task<List<Coach2StudMsgsReport>> CoachToStudMsgs()
        {
            var allCoaches = Services.MemberService.GetMembersByMemberType("coach1");
            List<List<Coach2StudMsgsReport>> result = new List<List<Coach2StudMsgsReport>>();

            foreach(var coach in allCoaches)
            {

                if(coach != null)
                {
                   var response = await messageService.GetAllMsgsFromCoach(coach.Key);
                    if(response != null)
                    {
                        result.Add(response);
                    }
                }
                
            }
            var finalResultList = result.SelectMany(x => x).ToList();
            return finalResultList;
        }

        [HttpGet]
        public List<ForretningsPlanService.BPMonthly> GetBusinessPlans()
        {
            var forretningsPlanService = new ForretningsPlanService();
            return forretningsPlanService.GetAllPerMonth();
        }


        #region Active Students
        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveStudents> GetWeeklyActiveStudents()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastWeek = nowDt.AddDays(-7);
            List<ActiveStudents> activeStudentsList= new List<ActiveStudents>();
            var dbActiveStudent = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastWeek);
            foreach (var student in dbActiveStudent)
            {
                var studentProfile = Services.MemberService.GetById(student.MemberId);
                if (studentProfile != null && studentProfile.ContentType.Alias.Equals("student") && !activeStudentsList.Any(x => x.Id == student.MemberId))
                {
                    int studentLogins = dbActiveStudent.Where(x => x.MemberId == student.MemberId).Count();
                        activeStudentsList.Add(new ActiveStudents
                        {
                            Id = studentProfile.Id,
                            Name = studentProfile.Name,
                            Email = studentProfile.Email,
                            IsApproved = studentProfile.IsApproved,
                            NumberOfLogins = studentLogins,
                            LastLoginDate = studentProfile.LastLoginDate
                        });
                    
                }
            }
            return activeStudentsList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveStudents> GetMonthlyActiveStudents()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastMonth = nowDt.AddMonths(-1);
            List<ActiveStudents> activeStudentsList = new List<ActiveStudents>();
            var dbActiveStudent = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastMonth);
            foreach (var student in dbActiveStudent)
            {
                var studentProfile = Services.MemberService.GetById(student.MemberId);
                if (studentProfile != null && studentProfile.ContentType.Alias.Equals("student") && !activeStudentsList.Any(x => x.Id == student.MemberId))
                {
                    int studentLogins = dbActiveStudent.Where(x => x.MemberId == student.MemberId).Count();
                    activeStudentsList.Add(new ActiveStudents
                    {
                        Id = studentProfile.Id,
                        Name = studentProfile.Name,
                        Email = studentProfile.Email,
                        IsApproved = studentProfile.IsApproved,
                        NumberOfLogins = studentLogins,
                        LastLoginDate = studentProfile.LastLoginDate
                    });

                }
            }
            return activeStudentsList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveStudents> GetYearlyActiveStudents()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastYear = nowDt.AddYears(-1);
            List<ActiveStudents> activeStudentsList = new List<ActiveStudents>();
            var dbActiveStudent = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastYear);
            foreach (var student in dbActiveStudent)
            {
                var studentProfile = Services.MemberService.GetById(student.MemberId);
                if (studentProfile != null && studentProfile.ContentType.Alias.Equals("student") && !activeStudentsList.Any(x => x.Id == student.MemberId))
                {
                    int studentLogins = dbActiveStudent.Where(x => x.MemberId == student.MemberId).Count();
                    activeStudentsList.Add(new ActiveStudents
                    {
                        Id = studentProfile.Id,
                        Name = studentProfile.Name,
                        Email = studentProfile.Email,
                        IsApproved = studentProfile.IsApproved,
                        NumberOfLogins = studentLogins,
                        LastLoginDate = studentProfile.LastLoginDate
                    });

                }
            }
            return activeStudentsList;
        }
        #endregion

        #region Active Coaches

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveCoaches> GetWeeklyActiveCoaches()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastWeek = nowDt.AddDays(-7);
            List<ActiveCoaches> activeCoachesList = new List<ActiveCoaches>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastWeek);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if(memberProfile != null && memberProfile.ContentType.Alias.Equals("coach1") && !activeCoachesList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activeCoachesList.Add(new ActiveCoaches
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsAvailable = !Convert.ToBoolean(memberProfile.GetValue("wwunavailable")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activeCoachesList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveCoaches> GetMonthlyActiveCoaches()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastMonth = nowDt.AddMonths(-1);
            List<ActiveCoaches> activeCoachesList = new List<ActiveCoaches>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastMonth);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if (memberProfile != null && memberProfile.ContentType.Alias.Equals("coach1") && !activeCoachesList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activeCoachesList.Add(new ActiveCoaches
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsAvailable = !Convert.ToBoolean(memberProfile.GetValue("wwunavailable")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activeCoachesList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActiveCoaches> GetYearlyActiveCoaches()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastYear = nowDt.AddMonths(-1);
            List<ActiveCoaches> activeCoachesList = new List<ActiveCoaches>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastYear);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if (memberProfile != null && memberProfile.ContentType.Alias.Equals("coach1") && !activeCoachesList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activeCoachesList.Add(new ActiveCoaches
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsAvailable = !Convert.ToBoolean(memberProfile.GetValue("wwunavailable")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activeCoachesList;
        }
        #endregion

        #region Active Partners
        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActivePartners> GetWeeklyActivePartners()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastWeek = nowDt.AddDays(-7);
            List<ActivePartners> activePartnersList = new List<ActivePartners>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastWeek);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if (memberProfile != null && memberProfile.ContentType.Alias.Equals("partner") && !activePartnersList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activePartnersList.Add(new ActivePartners
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsBinded = Convert.ToBoolean(memberProfile.GetValue("binding")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activePartnersList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActivePartners> GetMonthlyActivePartners()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastWeek = nowDt.AddMonths(-1);
            List<ActivePartners> activePartnersList = new List<ActivePartners>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastWeek);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if (memberProfile != null && memberProfile.ContentType.Alias.Equals("partner") && !activePartnersList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activePartnersList.Add(new ActivePartners
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsBinded = Convert.ToBoolean(memberProfile.GetValue("binding")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activePartnersList;
        }

        [HttpGet]
        //[Umbraco.Web.WebApi.MemberAuthorize(AllowMembers = allowedMembers)]
        public List<ActivePartners> GetYearlyActivePartners()
        {
            DateTime nowDt = DateTime.Now;
            DateTime lastWeek = nowDt.AddYears(-1);
            List<ActivePartners> activePartnersList = new List<ActivePartners>();
            var AllActiveMembers = _unitOfWork.AllActiveMembersRepository.GetAll().Where(x => x.LoginDate >= lastWeek);
            foreach (var activeMember in AllActiveMembers)
            {
                var memberProfile = Services.MemberService.GetById(activeMember.MemberId);
                if (memberProfile != null && memberProfile.ContentType.Alias.Equals("partner") && !activePartnersList.Any(x => x.Id == activeMember.MemberId))
                {
                    int coachLogins = AllActiveMembers.Where(x => x.MemberId == activeMember.MemberId).Count();
                    activePartnersList.Add(new ActivePartners
                    {
                        Id = memberProfile.Id,
                        Name = memberProfile.Name,
                        Email = memberProfile.Email,
                        IsApproved = memberProfile.IsApproved,
                        IsBinded = Convert.ToBoolean(memberProfile.GetValue("binding")),
                        NumberOfLogins = coachLogins,
                        LastLoginDate = memberProfile.LastLoginDate

                    }); ;
                }

            }
            return activePartnersList;
        }

        #endregion

    }










    public class MemberData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime CreateDate { get; set; }

        public string ContentType { get; set; }

        public bool IsApproved { get; set; }

        public bool? IsAvailable { get; set; }

        public bool? IsBinded { get; set; }

        public string CVR { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string Mobile { get; set; }
    }

    public class ActiveStudents
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public int NumberOfLogins { get; set; }
        public DateTime LastLoginDate { get; set; }

    }

    public class ActiveCoaches
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailable { get; set; }
        public int NumberOfLogins { get; set; }
        public DateTime LastLoginDate { get; set; }

    }


    public class ActivePartners
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public bool IsBinded { get; set; }
        public int NumberOfLogins { get; set; }
        public DateTime LastLoginDate { get; set; }

    }
}