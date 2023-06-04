
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class ProductsController : BaseApiController
  {
    private readonly IGenericRepository<Product> _productsRepo;
    private readonly IGenericRepository<ProductType> _productTypesRepo;
    private readonly IGenericRepository<Pot> _potsRepo;
    private readonly IGenericRepository<Images> _imagesRepo;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productsRepo, IGenericRepository<ProductType> productTypesRepo, IGenericRepository<Pot> potsRepo, IGenericRepository<Images> imagesRepo, IMapper mapper)
    {
      _productsRepo = productsRepo;
      _productTypesRepo = productTypesRepo;
      _potsRepo = potsRepo;
      _imagesRepo = imagesRepo;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDTO>>> GetProductsAsync()
    {
      var spec = new ProductsWithTypesAndPotsAndImagesSpecification();
      var products = await _productsRepo.ListAsync(spec);
      return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDTO>> GetProductAsync(int id)
    {
      var spec = new ProductsWithTypesAndPotsAndImagesSpecification(id);
      var product = await _productsRepo.GetEntityWithSpec(spec);
      if (product == null) return NotFound(new ApiResponse(404));
      return _mapper.Map<Product, ProductToReturnDTO>(product);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypesAsync()
    {
      var productTypes = await _productTypesRepo.ListAllAsync();
      return Ok(productTypes);
    }

    [HttpGet("pots")]
    public async Task<ActionResult<IReadOnlyList<Pot>>> GetPotsAsync()
    {
      var pots = await _potsRepo.ListAllAsync();
      return Ok(pots);
    }

    [HttpGet("images")]
    public async Task<ActionResult<IReadOnlyList<Images>>> GetImagesAsync()
    {
      var images = await _imagesRepo.ListAllAsync();
      return Ok(images);
    }
  }
}
