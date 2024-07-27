﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadedProject2.Models;

namespace ThreadedProject2.Helpers
{
    public class DB
    {
        static public class Get
        {
            static int supLastId = 0;
            static int packProdLastId = 0;
            static int packLastId;

            /// <summary>
            /// Get packages
            /// </summary>
            /// <param name="id">Optional id parameter to query by PackageId</param>
            /// <returns></returns>
            static public List<Models.Package> Packages(int id = -1)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    if (id == -1)
                    {
                        var packages = context.Packages.Select(p => p);
                        return packages.ToList();
                    }
                    else
                    {
                        var packages = context.Packages.Where(p => p.PackageId == id);
                        return packages.ToList();
                    }
                }
            }

            /// <summary>
            /// Get products
            /// </summary>
            /// <param name="id">Optional id parameter to query by ProductId</param>
            /// <returns></returns>
            static public List<Product> Products(int id = -1)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    if (id == -1)
                    {
                        var products = context.Products.Select(p => p).OrderBy(p => p.ProductId);
                        return products.ToList();
                    }
                    else
                    {
                        var products = context.Products.Where(p => p.ProductId == id);
                        return products.ToList();
                    }
                }
            }

            /// <summary>
            /// Get Suppliers
            /// </summary>
            /// <param name="id">Optional id parameter to query by supplier id</param>
            /// <returns></returns>
            static public List<Supplier> Suppliers(int id = -1)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    if (id == -1)
                    {
                        var suppliers = context.Suppliers.Select(p => p).OrderBy(p => p.SupName);
                        return suppliers.ToList();
                    }
                    else
                    {
                        var suppliers = context.Suppliers.Where(p => p.SupplierId == id).OrderBy(p => p.SupName);
                        return suppliers.ToList();
                    }
                }
            }

            /// <summary>
            /// Get Product suppliers
            /// </summary>
            /// <param name="id">Optional id parameter to query by productid</param>
            /// <returns></returns>
            static public List<ProductsSupplier> ProductSuppliers(int id = -1)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    if (id == -1)
                    {
                        var prodSuppliers = context.ProductsSuppliers.Select(p => p).OrderBy(p => p.ProductSupplierId);
                        return prodSuppliers.ToList();
                    }
                    else
                    {
                        var prodSuppliers = context.ProductsSuppliers.Where(p => p.ProductSupplierId == id).OrderBy(p => p.ProductSupplierId);
                        return prodSuppliers.ToList();
                    }
                }
            }

            /// <summary>
            /// Get Supplier contacts
            /// </summary>
            /// <param name="lstData">ListBox lstdata to get supplier id</param>
            /// <param name="useLast">To show all supplier contacts, false, else to reference only previous supplier, true reference the id of the previous menu selection</param>
            /// <returns></returns>
            static public List<SupplierContact> SupplierContacts(int id, bool useLast = false)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    if (!useLast) supLastId = id;
                    return context.SupplierContacts.Where(p => p.SupplierId == (useLast ? supLastId : id)).ToList();
                }
            }

            /// <summary>
            /// Get package product suppliers
            /// </summary>
            /// <param name="lstData">Listbox lstdata to get Package ID</param>
            /// <param name="useLast">To show all Package product supplies, false, else to reference only previous package, true. references the id of the previous menu selection</param>
            /// <returns></returns>
            static public (List<ProductsSupplier>, int) PackageProductSupplies(ListBox lstData, bool useLast = false)
            {
                using (Models.TravelExpertsContext context = new TravelExpertsContext())
                {
                    int packId = 0;
                    if (!useLast)
                    {
                        packId = Id.GetId(lstData);
                        packProdLastId = packId;
                    }

                    var package = context.Packages
                                     .Include(p => p.ProductSuppliers)
                                     .SingleOrDefault(p => p.PackageId == (useLast ? packProdLastId : packId));

                    return (package.ProductSuppliers.ToList(), packProdLastId);
                }

            }
        }

        public class Remove
        {
            static public void Package(int id)
            {
                using (Models.TravelExpertsContext context = new Models.TravelExpertsContext())
                {
                    Models.Package package = DB.Get.Packages(id).FirstOrDefault();
                    context.Packages.Remove(package);
                    context.SaveChanges();
                }
            }

            static public void ProductSupplies(List<ProductsSupplier> productSuppliers)
            {
                using (Models.TravelExpertsContext context = new Models.TravelExpertsContext())
                {
                    foreach (var ps in productSuppliers)
                    {
                        context.ProductsSuppliers.Remove(ps);
                    }

                    context.SaveChanges();
                }
            }

            static public void Suppliers(List<Supplier> suppliers)
            {
                using (Models.TravelExpertsContext context = new Models.TravelExpertsContext())
                {
                    foreach (var s in suppliers)
                    {
                        context.Suppliers.Remove(s);
                    }
                    context.SaveChanges();
                }
            }

            static public void Products(int id)
            {
                using (Models.TravelExpertsContext context = new Models.TravelExpertsContext())
                {
                    var product = context.Products.Where(p => p.ProductId == id).FirstOrDefault();
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
            }

            static public void PackageProductSupply(TravelExpertsContext context, List<Package> packages)
            {

                foreach (var package in packages)
                {
                    foreach (var ps in context.ProductsSuppliers)
                    {   
                        if (package.ProductSuppliers.Any(x => x.ProductSupplierId == ps.ProductSupplierId))
                        {
                            package.ProductSuppliers.Remove(ps);
                        }
                    }
                }
                    context.SaveChanges();
            }

            static public void SupplierContacts(List<SupplierContact> supplierContacts)
            {
                using (Models.TravelExpertsContext context = new Models.TravelExpertsContext())
                {
                    foreach (var s in supplierContacts)
                    {
                        context.SupplierContacts.Remove(s);
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
