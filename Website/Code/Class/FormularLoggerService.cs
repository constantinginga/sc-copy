using StartupCentral.Code.Model;
using StartupCentral.DAL.EntityModels;
using StartupCentral.DAL.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StartupCentral.Code.Class
{
    public class FormularLoggerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FormularLoggerService()
        {
            _unitOfWork = new UnitOfWork();
        }

        public async Task<Guid> Save(FormularLogger form)
        {
            var formularDb = new formularLogger()
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                CVRNumber = form.CVRNumber,
                Industry = form.Industry,
                Email = form.Email,
                Name = form.Name,
                PhoneNumber = form.PhoneNumber,
                Notes = form.Notes,
                Contacted = false,
                MailFlow = false,
                Read = false
            };

            //if(owner != null)
            //    formularDb.Owner = owner;

            _unitOfWork.FormularLoggerRepository.Insert(formularDb);
            await _unitOfWork.SaveAsync();
            return formularDb.Id;
        }

        public async Task<List<formularLogger>> GetListOfAllFormulars()
        {
            var res = await _unitOfWork.FormularLoggerRepository.WhereAsync();
            return res.ToList();
        }

        public async Task<List<formularLogger>> GetListOfUnreadFormulars()
        {
            var res = await _unitOfWork.FormularLoggerRepository.WhereAsync(x=>!x.Read);
            return res.ToList();
        }

        public async Task<Guid> UpdateContactedProperty(Guid recordKey, bool state)
        {
            var res = await _unitOfWork.FormularLoggerRepository.WhereAsync(x => x.Id == recordKey && x.Contacted == !state);

            res.ToList().ForEach(x =>
            {
                x.Contacted = state;
                _unitOfWork.FormularLoggerRepository.Update(x);
            });

            await _unitOfWork.SaveAsync();

            return res.First().Id;
        }

        public async Task<string> UpdateMailFlowProperty(Guid recordKey, bool state)
        {
            var res = await _unitOfWork.FormularLoggerRepository.WhereAsync(x => x.Id == recordKey && x.MailFlow == !state);

            if (res.Any())
            {
                res.ToList().ForEach(x =>
                {
                    x.MailFlow = state;
                    _unitOfWork.FormularLoggerRepository.Update(x);
                });

                await _unitOfWork.SaveAsync();

                return res.First().Email;
            }

            return null;        
        }

        public async Task MarkRead(Guid recordKey)
        {
            var unread = await _unitOfWork.FormularLoggerRepository.WhereAsync(x => x.Id == recordKey && !x.Read);
            unread.ToList().ForEach(x =>
            {
                x.Read = true;
                _unitOfWork.FormularLoggerRepository.Update(x);
            });
            await _unitOfWork.SaveAsync();            
        }


    }
}