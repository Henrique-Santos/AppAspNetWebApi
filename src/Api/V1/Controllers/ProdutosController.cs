using Api.Controllers;
using Api.Extensions;
using Api.ViewModels;
using AutoMapper;
using Business.Intefaces;
using Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(
            INotificador notificador,
            IUser user,
            IProdutoRepository produtoRepository,
            IProdutoService produtoService,
            IMapper mapper
        ) : base(notificador, user)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel is null) return NotFound();
            return produtoViewModel;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            var nomeImagem = Guid.NewGuid() + "_" + produtoViewModel.NomeImagem;
            if (!SubirArquivo(produtoViewModel.ImagemUpload, nomeImagem)) return RespostaCustomizada(produtoViewModel);
            produtoViewModel.NomeImagem = nomeImagem;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return RespostaCustomizada(produtoViewModel);
        }

        // Binder personalizado para envio de IFormFile e ViewModel dentro de um FormData. Compatível com .NET Core 3.1 ou superior (system.text.json)
        [HttpPost("adicionar-alternativo")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(
            [ModelBinder(BinderType = typeof(ProdutoModelBinder))] ProdutoImagemViewModel produtoViewModel
        )
        {
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            var prefixoImagem = Guid.NewGuid() + "_";
            if (!await SubirArquivoAlternativo(produtoViewModel.ImagemUpload, prefixoImagem))
                return RespostaCustomizada(produtoViewModel);
            produtoViewModel.NomeImagem = prefixoImagem + produtoViewModel.ImagemUpload.FileName;
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            return RespostaCustomizada(produtoViewModel);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotificarErro("Os ids informados não são iguais!");
                return RespostaCustomizada();
            }
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            var produtoAtualizacao = await ObterProduto(id);
            if (string.IsNullOrEmpty(produtoViewModel.NomeImagem))
                produtoViewModel.NomeImagem = produtoAtualizacao.NomeImagem;
            if (produtoViewModel.ImagemUpload != null)
            {
                var nomeImagem = Guid.NewGuid() + "_" + produtoViewModel.NomeImagem;
                if (!SubirArquivo(produtoViewModel.ImagemUpload, nomeImagem))
                    return RespostaCustomizada(ModelState);
                produtoAtualizacao.NomeImagem = nomeImagem;
            }
            produtoAtualizacao.FornecedorId = produtoViewModel.FornecedorId;
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            return RespostaCustomizada(produtoViewModel);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            if (produtoViewModel is null) return NotFound();
            await _produtoService.Remover(id);
            return RespostaCustomizada(produtoViewModel);
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }

        private bool SubirArquivo(string arquivo, string nomeImagem)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para esse produto");
                return false;
            }
            var imagemDataByteArray = Convert.FromBase64String(arquivo);
            var diretorioArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", nomeImagem);
            if (System.IO.File.Exists(diretorioArquivo))
            {
                NotificarErro("Já existe um arquivo com esse nome");
                return false;
            }
            System.IO.File.WriteAllBytes(diretorioArquivo, imagemDataByteArray);
            return true;
        }

        private async Task<bool> SubirArquivoAlternativo(IFormFile arquivo, string prefixoImagem)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para esse produto");
                return false;
            }
            var diretorioArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", prefixoImagem + arquivo.FileName);
            if (System.IO.File.Exists(diretorioArquivo))
            {
                NotificarErro("Já existe um arquivo com esse nome");
                return false;
            }
            using var stream = new FileStream(diretorioArquivo, FileMode.Create);
            await arquivo.CopyToAsync(stream);
            return true;
        }
    }
}