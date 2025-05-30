﻿using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    
    public class MarcaController : BaseApiController
    {
        private readonly IGenericRepository<Marca> _marcaRepository;

        public MarcaController (IGenericRepository<Marca> marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<Marca>> GetMarcaAll()
        {
            return Ok(await _marcaRepository.GetAllAsync());    
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Marca>> GetMarcaById(int id)
        {
            return await _marcaRepository.GetByIdAsync(id);
        }
    }
}
