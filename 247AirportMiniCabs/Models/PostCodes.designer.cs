﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlinTuriCab.Models
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="UKPostCodesPro_Data")]
	public partial class PostCodesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public PostCodesDataContext() :
        base(AlinTuriCab.Helpers.StaticHelper.IsOnline ? @"Data Source=localhost;Initial Catalog=UKPostCodesPro_Data;Integrated Security=false;User ID=alindracula;Password=Muaz0077" : @"Data Source=.\SQLEXPRESS;Initial Catalog=UKPostCodesPro_Data;Integrated Security=True", mappingSource)
    {
			OnCreated();
		}
		
		public PostCodesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PostCodesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PostCodesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PostCodesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<UKAddress> UKAddresses
		{
			get
			{
				return this.GetTable<UKAddress>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.UKAddress")]
	public partial class UKAddress
	{
		
		private string _PCUnit;
		
		private string _PCSector;
		
		private string _PCDistrict;
		
		private string _PCArea;
		
		private string _PQI;
		
		private string _Eastings;
		
		private string _Northings;
		
		private string _Latitude;
		
		private string _Longitude;
		
		private string _GridRef;
		
		private string _Street;
		
		private string _SQI;
		
		private string _Town;
		
		private string _TownCode;
		
		private string _Ward;
		
		private string _WardCode;
		
		private string _District;
		
		private string _DistrictCode;
		
		private string _County;
		
		private string _CountyCode;
		
		private string _Region;
		
		private string _RegionCode;
		
		private string _Country;
		
		private string _CountryCode;
		
		private string _NHSCode;
		
		private string _NHSRegionCode;
		
		public UKAddress()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PCUnit", DbType="NVarChar(255)")]
		public string PCUnit
		{
			get
			{
				return this._PCUnit;
			}
			set
			{
				if ((this._PCUnit != value))
				{
					this._PCUnit = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PCSector", DbType="NVarChar(255)")]
		public string PCSector
		{
			get
			{
				return this._PCSector;
			}
			set
			{
				if ((this._PCSector != value))
				{
					this._PCSector = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PCDistrict", DbType="NVarChar(255)")]
		public string PCDistrict
		{
			get
			{
				return this._PCDistrict;
			}
			set
			{
				if ((this._PCDistrict != value))
				{
					this._PCDistrict = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PCArea", DbType="NVarChar(255)")]
		public string PCArea
		{
			get
			{
				return this._PCArea;
			}
			set
			{
				if ((this._PCArea != value))
				{
					this._PCArea = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PQI", DbType="NVarChar(255)")]
		public string PQI
		{
			get
			{
				return this._PQI;
			}
			set
			{
				if ((this._PQI != value))
				{
					this._PQI = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Eastings", DbType="NVarChar(255)")]
		public string Eastings
		{
			get
			{
				return this._Eastings;
			}
			set
			{
				if ((this._Eastings != value))
				{
					this._Eastings = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Northings", DbType="NVarChar(255)")]
		public string Northings
		{
			get
			{
				return this._Northings;
			}
			set
			{
				if ((this._Northings != value))
				{
					this._Northings = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Latitude", DbType="NVarChar(255)")]
		public string Latitude
		{
			get
			{
				return this._Latitude;
			}
			set
			{
				if ((this._Latitude != value))
				{
					this._Latitude = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Longitude", DbType="NVarChar(255)")]
		public string Longitude
		{
			get
			{
				return this._Longitude;
			}
			set
			{
				if ((this._Longitude != value))
				{
					this._Longitude = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_GridRef", DbType="NVarChar(255)")]
		public string GridRef
		{
			get
			{
				return this._GridRef;
			}
			set
			{
				if ((this._GridRef != value))
				{
					this._GridRef = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Street", DbType="NVarChar(255)")]
		public string Street
		{
			get
			{
				return this._Street;
			}
			set
			{
				if ((this._Street != value))
				{
					this._Street = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SQI", DbType="NVarChar(255)")]
		public string SQI
		{
			get
			{
				return this._SQI;
			}
			set
			{
				if ((this._SQI != value))
				{
					this._SQI = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Town", DbType="NVarChar(255)")]
		public string Town
		{
			get
			{
				return this._Town;
			}
			set
			{
				if ((this._Town != value))
				{
					this._Town = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TownCode", DbType="NVarChar(255)")]
		public string TownCode
		{
			get
			{
				return this._TownCode;
			}
			set
			{
				if ((this._TownCode != value))
				{
					this._TownCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Ward", DbType="NVarChar(255)")]
		public string Ward
		{
			get
			{
				return this._Ward;
			}
			set
			{
				if ((this._Ward != value))
				{
					this._Ward = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WardCode", DbType="NVarChar(255)")]
		public string WardCode
		{
			get
			{
				return this._WardCode;
			}
			set
			{
				if ((this._WardCode != value))
				{
					this._WardCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_District", DbType="NVarChar(255)")]
		public string District
		{
			get
			{
				return this._District;
			}
			set
			{
				if ((this._District != value))
				{
					this._District = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DistrictCode", DbType="NVarChar(255)")]
		public string DistrictCode
		{
			get
			{
				return this._DistrictCode;
			}
			set
			{
				if ((this._DistrictCode != value))
				{
					this._DistrictCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_County", DbType="NVarChar(255)")]
		public string County
		{
			get
			{
				return this._County;
			}
			set
			{
				if ((this._County != value))
				{
					this._County = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CountyCode", DbType="NVarChar(255)")]
		public string CountyCode
		{
			get
			{
				return this._CountyCode;
			}
			set
			{
				if ((this._CountyCode != value))
				{
					this._CountyCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Region", DbType="NVarChar(255)")]
		public string Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				if ((this._Region != value))
				{
					this._Region = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegionCode", DbType="NVarChar(255)")]
		public string RegionCode
		{
			get
			{
				return this._RegionCode;
			}
			set
			{
				if ((this._RegionCode != value))
				{
					this._RegionCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Country", DbType="NVarChar(255)")]
		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				if ((this._Country != value))
				{
					this._Country = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CountryCode", DbType="NVarChar(255)")]
		public string CountryCode
		{
			get
			{
				return this._CountryCode;
			}
			set
			{
				if ((this._CountryCode != value))
				{
					this._CountryCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NHSCode", DbType="NVarChar(255)")]
		public string NHSCode
		{
			get
			{
				return this._NHSCode;
			}
			set
			{
				if ((this._NHSCode != value))
				{
					this._NHSCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NHSRegionCode", DbType="NVarChar(255)")]
		public string NHSRegionCode
		{
			get
			{
				return this._NHSRegionCode;
			}
			set
			{
				if ((this._NHSRegionCode != value))
				{
					this._NHSRegionCode = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
