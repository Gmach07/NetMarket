﻿using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    
    public class CategoriaController : BaseApiController
    {
        private readonly IGenericRepository<Categoria> _categoriaRepository;

        public CategoriaController(IGenericRepository<Categoria> categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Categoria>>> GetCategoriaAll()
        {
            return Ok(await _categoriaRepository.GetAllAsync());
        }

        [HttpGet ("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoriaById(int id)
        {
            return await _categoriaRepository.GetByIdAsync(id);
        }
    } 
}
