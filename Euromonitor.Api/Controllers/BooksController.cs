﻿using AutoMapper;
using Euromonitor.DataAccess.Data.Repository.IRepository;
using Euromonitor.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Euromonitor.Api.Controllers
{
    public class BooksController : BaseApiController
    {
        //Unit of work to access DB
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public BooksController(IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        //Get all Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            //Below we have to use async version of ToList
            var books = await _unitOfWork.Book.GetBooksAsync();

            //Map to DTO
            //Source: Book
            //Output: <IEnumerable<BookDto>>

            var booksToReturn = _mapper.Map<IEnumerable<BookDto>>(books);

            //Wrap result in an OK response
            return Ok(booksToReturn);
        }

        //Get specific book
        //api/books/3
        [HttpGet("{bookname}")]
        public async Task<ActionResult<BookDto>> GetBook(string bookname)
        {
            var book = await _unitOfWork.Book.GetBookByBookNameAsync(bookname);

            //FindAsync method rather than Find. Map
            return _mapper.Map<BookDto>(book);
        }

        //Update Book
        [HttpPut]
        public async Task<ActionResult> UpdateBook(BookUpdateDto bookUpdateDto)
        {

            //Get book from DB by Id
            var book = await _unitOfWork.Book.GetBookByIdAsync(bookUpdateDto.Id);

            //Map the input Dto to our Book class
            _mapper.Map(bookUpdateDto, book);

            //Book object is flagged as being updated by EF
            _unitOfWork.Book.Update(book);

            //Persist changes to DB
            if (await _unitOfWork.Book.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Failed to update Book.");
            }

        }
    }
}
