using Api.ViewModels;
using AutoMapper;
using Business.Models;

namespace Api.Configurations
{
    public class AutomapperConfiguration : Profile
    {
        public AutomapperConfiguration()
        {
            CreateMap<Fornecedor, FornecedorViewModel>()
                .ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>()
                .ReverseMap();
            CreateMap<ProdutoViewModel, Produto>()
                .ForMember(x => x.Imagem, y => y.MapFrom(z => z.NomeImagem));
            CreateMap<ProdutoImagemViewModel, Produto>()
                .ForMember(x => x.Imagem, y => y.MapFrom(z => z.NomeImagem))
                .ReverseMap();
            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(x => x.NomeImagem, y => y.MapFrom(z => z.Imagem))
                .ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}