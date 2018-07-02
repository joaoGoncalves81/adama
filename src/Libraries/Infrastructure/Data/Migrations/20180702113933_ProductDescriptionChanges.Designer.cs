﻿// <auto-generated />
using System;
using ApplicationCore.Entities;
using ApplicationCore.Entities.OrderAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(DamaContext))]
    [Migration("20180702113933_ProductDescriptionChanges")]
    partial class ProductDescriptionChanges
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("Relational:Sequence:.catalog_brand_hilo", "'catalog_brand_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.catalog_hilo", "'catalog_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("Relational:Sequence:.catalog_type_hilo", "'catalog_type_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApplicationCore.Entities.Basket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BuyerId");

                    b.HasKey("Id");

                    b.ToTable("Baskets");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketDetailItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BasketItemId");

                    b.Property<int>("CatalogAttributeId");

                    b.HasKey("Id");

                    b.HasIndex("BasketItemId");

                    b.HasIndex("CatalogAttributeId");

                    b.ToTable("BasketDetailItem");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BasketId");

                    b.Property<int>("CatalogItemId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice");

                    b.HasKey("Id");

                    b.HasIndex("BasketId");

                    b.ToTable("BasketItem");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CatalogItemId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<decimal?>("Price");

                    b.Property<int?>("ReferenceCatalogItemId");

                    b.Property<string>("Sku")
                        .HasMaxLength(255);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("ReferenceCatalogItemId");

                    b.ToTable("CatalogAttribute");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CatalogItemId");

                    b.Property<int>("CategoryId");

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_brand_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("IllustrationTypeId");

                    b.Property<byte[]>("Image");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("IllustrationTypeId");

                    b.ToTable("CatalogIllustration");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<bool>("CanCustomize")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<int>("CatalogIllustrationId");

                    b.Property<int>("CatalogTypeId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsFeatured");

                    b.Property<bool>("IsNew");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri");

                    b.Property<decimal>("Price");

                    b.Property<bool>("ShowOnShop");

                    b.Property<string>("Sku")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("CatalogIllustrationId");

                    b.HasIndex("CatalogTypeId");

                    b.ToTable("Catalog");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CatalogItemId");

                    b.Property<bool>("IsActive");

                    b.Property<int>("Order");

                    b.Property<string>("PictureUri")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CatalogItemId");

                    b.ToTable("CatalogPicture");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "catalog_type_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<int>("DeliveryTimeMax")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(3);

                    b.Property<int>("DeliveryTimeMin")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(2);

                    b.Property<int>("DeliveryTimeUnit")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("CatalogType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.Property<int>("CatalogTypeId");

                    b.Property<int>("CategoryId");

                    b.HasKey("CatalogTypeId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CatalogTypeCategory");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<int?>("ParentId");

                    b.Property<string>("Position")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10)
                        .HasDefaultValue("left");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttachFileName");

                    b.Property<string>("BuyerContact")
                        .IsRequired();

                    b.Property<string>("BuyerId")
                        .IsRequired();

                    b.Property<string>("BuyerName")
                        .IsRequired();

                    b.Property<string>("Colors");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<DateTimeOffset>("OrderDate");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("CustomizeOrder");
                });

            modelBuilder.Entity("ApplicationCore.Entities.IllustrationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("IllustrationType");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BuyerId");

                    b.Property<DateTimeOffset>("OrderDate");

                    b.Property<int>("OrderState")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<long?>("SalesInvoiceId");

                    b.Property<string>("SalesInvoiceNumber")
                        .HasMaxLength(255);

                    b.Property<long?>("SalesPaymentId");

                    b.Property<decimal>("ShippingCost");

                    b.Property<int?>("TaxNumber");

                    b.Property<bool>("UseBillingSameAsShipping")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("OrderId");

                    b.Property<decimal>("UnitPrice");

                    b.Property<int>("Units");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItemDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttributeName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("AttributeType");

                    b.Property<int>("OrderItemId");

                    b.HasKey("Id");

                    b.HasIndex("OrderItemId");

                    b.ToTable("OrderItemDetails");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int>("Type");

                    b.Property<string>("Value")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("ShopConfig");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContentText");

                    b.Property<string>("HeadingText");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LinkButtonText");

                    b.Property<string>("LinkButtonUri");

                    b.Property<string>("PictureUri")
                        .HasMaxLength(255);

                    b.Property<int>("ShopConfigId");

                    b.HasKey("Id");

                    b.HasIndex("ShopConfigId");

                    b.ToTable("ShopConfigDetail");
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketDetailItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.BasketItem", "BasketItem")
                        .WithMany("Details")
                        .HasForeignKey("BasketItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.CatalogAttribute", "CatalogAttribute")
                        .WithMany()
                        .HasForeignKey("CatalogAttributeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.BasketItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Basket")
                        .WithMany("Items")
                        .HasForeignKey("BasketId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogAttribute", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogAttributes")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.CatalogItem", "ReferenceCatalogItem")
                        .WithMany("ReferenceCatalogAttributes")
                        .HasForeignKey("ReferenceCatalogItemId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogIllustration", b =>
                {
                    b.HasOne("ApplicationCore.Entities.IllustrationType", "IllustrationType")
                        .WithMany()
                        .HasForeignKey("IllustrationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogIllustration", "CatalogIllustration")
                        .WithMany()
                        .HasForeignKey("CatalogIllustrationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("CatalogItems")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogPicture", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogItem", "CatalogItem")
                        .WithMany("CatalogPictures")
                        .HasForeignKey("CatalogItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.CatalogTypeCategory", b =>
                {
                    b.HasOne("ApplicationCore.Entities.CatalogType", "CatalogType")
                        .WithMany("Categories")
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.Category", "Category")
                        .WithMany("CatalogTypes")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.Category", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Category", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.CustomizeOrder", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("CustomizeOrderId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("CatalogItemId");

                            b1.Property<string>("PictureUri");

                            b1.Property<string>("ProductName");

                            b1.ToTable("CustomizeOrder");

                            b1.HasOne("ApplicationCore.Entities.CustomizeOrder")
                                .WithOne("ItemOrdered")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "CustomizeOrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.Order", b =>
                {
                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "BillingToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("Name");

                            b1.Property<int?>("PhoneNumber");

                            b1.Property<string>("PostalCode");

                            b1.Property<string>("Street");

                            b1.ToTable("Orders");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                                .WithOne("BillingToAddress")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.Address", "ShipToAddress", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("City");

                            b1.Property<string>("Country");

                            b1.Property<string>("Name");

                            b1.Property<int?>("PhoneNumber");

                            b1.Property<string>("PostalCode");

                            b1.Property<string>("Street");

                            b1.ToTable("Orders");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                                .WithOne("ShipToAddress")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.Address", "OrderId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItem", b =>
                {
                    b.HasOne("ApplicationCore.Entities.OrderAggregate.Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId");

                    b.OwnsOne("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "ItemOrdered", b1 =>
                        {
                            b1.Property<int>("OrderItemId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("CatalogItemId");

                            b1.Property<string>("PictureUri");

                            b1.Property<string>("ProductName");

                            b1.ToTable("OrderItems");

                            b1.HasOne("ApplicationCore.Entities.OrderAggregate.OrderItem")
                                .WithOne("ItemOrdered")
                                .HasForeignKey("ApplicationCore.Entities.OrderAggregate.CatalogItemOrdered", "OrderItemId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.OrderAggregate.OrderItemDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.OrderAggregate.OrderItem", "OrderItem")
                        .WithMany("Details")
                        .HasForeignKey("OrderItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.ShopConfigDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ShopConfig", "ShopConfig")
                        .WithMany("Details")
                        .HasForeignKey("ShopConfigId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
