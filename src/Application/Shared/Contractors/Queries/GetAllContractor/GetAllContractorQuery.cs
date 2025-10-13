namespace ContractorBackend.Application.Contractors.Queries.GetAllContractor
{
    //public class GetAllContractorQuery :
    //     SearchQueryRequest, IRequest<SearchQueryResponse<ContractorDto>>
    //{
    //}

    //public class GetAllContractorQueryHandler : IRequestHandler<GetAllContractorQuery,
    //              SearchQueryResponse<ContractorDto>>
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IRepository<Contractor> _repository;
    //    private readonly IApplicationDbContext _dbContext;
    //    public GetAllContractorQueryHandler(IMapper mapper, IRepository<Contractor> repository, IApplicationDbContext dbContext)
    //    {
    //        _mapper = mapper;
    //        _repository = repository;
    //        _dbContext = dbContext;
    //    }
    //    public async Task<SearchQueryResponse<ContractorDto>> Handle(GetAllContractorQuery request, CancellationToken cancellationToken)
    //    {
    //        var query = _repository.GetAllAsNoTracking()
    //            .OrderByDescending(x => EF.Property<DateTimeOffset?>(x, "CreatedDateTime"))
    //            .ProjectTo<ContractorDto>(_mapper.ConfigurationProvider);

    //        QueryablePaging<ContractorDto> qp = await query.GridifyQueryableAsync<ContractorDto>(request, null, cancellationToken);
    //        Paging<ContractorDto> result = new(qp.Count, qp.Query.ToList());

    //        return new SearchQueryResponse<ContractorDto>(request, result);
    //    }
    //}
}
