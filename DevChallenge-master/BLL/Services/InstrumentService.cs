using AutoMapper;
using BLL.DTO;
using DAL.Context;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class InstrumentService
    {
        IMapper mapper;
        InstrumentRepository repository;
        public InstrumentService(FinancialContext context)
        {
            repository = new InstrumentRepository(context);

            MapperConfiguration config = new MapperConfiguration(x =>
            {
                x.CreateMap<Instrument, InstrumentDTO>();
                x.CreateMap<InstrumentDTO, Instrument>();
            });

            mapper = new Mapper(config);
        }

        public async Task<InstrumentDTO> Get(int id) => mapper.Map<Instrument, InstrumentDTO>(await repository.GetAsync(id));
        
        public IEnumerable<InstrumentDTO> GetAll() => mapper.Map<IEnumerable<Instrument>,
                                                        IEnumerable<InstrumentDTO>>(repository.GetAll());

        public async Task Update(InstrumentDTO inst)
        {
            await repository.UpdateAsync(mapper.Map<InstrumentDTO, Instrument>(inst));
            await repository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await repository.DeleteAsync(await repository.GetAsync(id));
            await repository.SaveChangesAsync();
        }

        public async Task Add(InstrumentDTO inst) 
        {
            await repository.AddAsync(mapper.Map<InstrumentDTO, Instrument>(inst));
            await repository.SaveChangesAsync();
        }
    }
}
