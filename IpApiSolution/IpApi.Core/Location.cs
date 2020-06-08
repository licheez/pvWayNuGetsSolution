﻿using System.Collections.Generic;
using System.Linq;

namespace pvWay.IpApi.Core
{
    internal class Location : ILocation
    {
        public int GeoNameId { get; }
        public string Capital { get; }
        public IEnumerable<ILanguage> Languages { get; }
        public string CountryFlagUrl { get; }
        public string CountryFlagEmoji { get; }
        public string CountryFlagEmojiUnicode { get; }
        public string CallingCode { get; }
        public bool EuroMember { get; }

        public Location(dynamic rd)
        {
            GeoNameId = rd.geoname_id;
            Capital = rd.capital;
            IEnumerable<dynamic> rdLanguages = rd.languages;
            Languages = rdLanguages
                .Select(x => new Language(x));
            CountryFlagUrl = rd.country_flag;
            CountryFlagEmoji = rd.country_flag_emoji;
            CountryFlagEmojiUnicode = rd.country_flag_emoji_unicode;
            CallingCode = rd.calling_code;
            EuroMember = rd.is_eu;
        }
    }
}