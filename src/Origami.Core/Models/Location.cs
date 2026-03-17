using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Origami.Core.Models
{
    [Owned]
    public class Location :
        IChanged
    {
        protected string _city = string.Empty;
        protected string _country = string.Empty;
        protected string _countryCode = string.Empty;
        protected float _latitude;
        protected float _longitude;
        protected string _region = string.Empty;
        protected string _regionCode = string.Empty;
        protected string _timeZone = string.Empty;
        protected int? _timeZoneOffset = 0;
        protected TimeSpan _utcOffset = TimeSpan.Zero;
        protected string _zipCode = string.Empty;

        public Location() : base()
        {

        }

        public event EventHandler<PropertyChangedEventArgs> Changed = (sender, e) => { };

        /// <summary>
        /// City
        /// </summary>
        [StringLength(200)]
        public string City
        {
            get => _city;
            set => this.Set(ref _city, value, Changed);
        }

        /// <summary>
        /// Country Name
        /// </summary>
        [StringLength(100)]
        public string Country
        {
            get => _country;
            set => this.Set(ref _country, value, Changed);
        }

        /// <summary>
        /// Country Code
        /// </summary>
        [StringLength(50)]
        public string CountryCode
        {
            get => _countryCode;
            set => this.Set(ref _countryCode, value, Changed);
        }

        /// <summary>
        /// Latitude
        /// </summary>
        public float Latitude
        {
            get => _latitude;
            set => this.Set(ref _latitude, value, Changed);
        }

        /// <summary>
        /// Longitude
        /// </summary>
        public float Longitude
        {
            get => _longitude;
            set => this.Set(ref _longitude, value, Changed);
        }

        [NotMapped]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Region Name
        /// </summary>
        [StringLength(100)]
        public string Region
        {
            get => _region;
            set => this.Set(ref _region, value, Changed);
        }

        /// <summary>
        /// Region Code
        /// </summary>
        [StringLength(50)]
        public string RegionCode
        {
            get => _regionCode;
            set => this.Set(ref _regionCode, value, Changed);
        }

        /// <summary>
        /// Time Zone
        /// </summary>
        [StringLength(20)]
        public string TimeZone
        {
            get => _timeZone;
            set => this.Set(ref _timeZone, value, Changed);
        }

        /// <summary>
        /// Time Zone Offset in minutes from UTC
        /// </summary>
        public int? TimeZoneOffset
        {
            get => _timeZoneOffset;
            set => this.Set(ref _timeZoneOffset, value, Changed);
        }

        /// <summary>
        /// Zip Code
        /// </summary>
        [StringLength(20)]
        public string ZipCode
        {
            get => _zipCode;
            set => this.Set(ref _zipCode, value, Changed);
        }
    }
}
