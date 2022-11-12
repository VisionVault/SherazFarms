using TMS.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS
{
    public class TMSContext : IdentityDbContext<User>
    {
        public TMSContext(DbContextOptions option) : base(option)
        {

        }
        public virtual DbSet<FSL> FSLs { get; set; }
        public virtual DbSet<CTL> CTLs { get; set; }
        public virtual DbSet<GAL> GALs { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountDetail> AccountDetails { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarBrand> CarBrands { get; set; }
        public virtual DbSet<CarEngine> CarEngines { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<LedgerDetail> LedgerDetails { get; set; }
        public virtual DbSet<DocType> DocTypes { get; set; }
        public virtual DbSet<TransactionStatus> TransactionStatuses { get; set; }
        public virtual DbSet<TradeFirm> TradeFirms { get; set; }
        public virtual DbSet<UOM> UOMs { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ProductLine> ProductLines { get; set; }
        public virtual DbSet<PaymentTerm> PaymentTerms { get; set; }
        public virtual DbSet<PendingReceipt> PendingReceipts { get; set; }
        public virtual DbSet<CustomerCar> CustomerCars { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
    }
}
