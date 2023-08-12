﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldCitiesAPI.Data.Models
{
    [Table("Countries")]
    [Index(nameof(Name))]
    [Index(nameof(ISO2))]
    [Index(nameof(ISO3))]
    public class Country
    {
        #region Properties

        ///<summary>
        ///The unique id and primary key for this country
        /// </summary>

        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Country name (in UTF8-Format
        /// </summary>

        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-2 FOrmat)
        /// </summary>
        public string ISO2 { get; set; } = null!;


        /// <summary>
        /// Country code (in ISO 3166-1 ALPHA-3 FOrmat)
        /// </summary>
        public string ISO3 { get; set; } = null!;
        #endregion

        #region Navigation Properties
        ///<summary>
        /// A collection of all the cities related to this country
        /// </summary>
        public ICollection<City>? Cities { get; set; } = null!;

        #endregion
    }
}