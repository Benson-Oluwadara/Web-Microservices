﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace mango.web.frontend.Models.WebDTO
{
    public class CartDetails
    {
      
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
       
        public CartHeader CartHeader { get; set; }
        public int ProductId { get; set; }
      
        public ProductDTO Product { get; set; }
        public int Count { get; set; }

    }
}
