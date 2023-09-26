using Api.Controllers;
using Api.Extensions;
using Api.ViewModels;
using AutoMapper;
using Business.Intefaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fornecedores")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IEnderecoRepository enderecoRepository,
            IFornecedorService fornecedorService,
            IMapper mapper,
            INotificador notificador,
            IUser user
        ) : base(notificador, user)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return Ok(fornecedores);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorComProdutosEEndereco(id);
            if (fornecedor is null) return NotFound();
            return Ok(fornecedor);
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));
            return RespostaCustomizada(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return BadRequest();
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));
            return RespostaCustomizada(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorComEndereco(id);
            if (fornecedorViewModel == null) return NotFound();
            await _fornecedorService.Remover(id);
            return RespostaCustomizada();
        }

        [HttpGet("endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado não é o mesmo que foi passado na query");
                return RespostaCustomizada(enderecoViewModel);
            }
            if (!ModelState.IsValid) return RespostaCustomizada(ModelState);
            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));
            return RespostaCustomizada(enderecoViewModel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorComProdutosEEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorComEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }
}