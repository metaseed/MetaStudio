using AmazedSaint.Elastic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmazedSaint.Elastic.Lib;
using System.Diagnostics;


//ElasticObject - http://elasticobject.codeplex.com

//Created by Anoop - http://amazedsaint.com 
//Ask questions at http://twitter.com/amazedsaint
//Warning: Totally unsupported, use at your own risk :)

//Quick Introduction here - http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html
//A demo from Friedman here - http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html

//For more, see/run the tests below

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for DynamicExtensionsTest and is intended
    ///to contain all DynamicExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DynamicExtensionsTest
    {

        /// <summary>
        /// A private helper method
        /// </summary>
        /// <returns></returns>
        private  dynamic CreateStoreObject()
        {
            dynamic store = new ElasticObject("Store");
            store.Name = "Acme Store";
            store.Location.Address = "West Avenue, Heaven Street Road, LA";
            store.Products.Count = 2;

            store.Owner.FirstName = "Jack";
            store.Owner.SecondName = "Jack";

            //try to set the internal content for an element
            store.Owner <<= "this is some internal content for owner";

            //Add a new product
            var p1 = store.Products.Product();
            p1.Name = "Acme Floor Cleaner";
            p1.Price = 20;

            //Add another product
            var p2 = store.Products.Product();
            p2.Name = "Acme Bun";
            p2.Price = 22;

            return store;

        }


        [Description("Test to verify the Object creation"), TestMethod()]
        public void Check_The_Object_Get_Created() 
        {
            CreateStoreObject();
        }



        [Description("Test to verify the Xml creation using '>' or format converter operator"),TestMethod()]
        public void Check_The_Object_Create_And_Consume_Xml()
        {
            var store = CreateStoreObject();
            XElement el = store > FormatType.Xml;
            dynamic storeClone = el.ToElastic();
            XElement elCopy = storeClone > FormatType.Xml;
            Assert.AreEqual(el.ToString(), elCopy.ToString());
        }


        [Description("Test to verify the indexer returns all children when passing a null"),TestMethod()]
        public void Check_The_Object_Indexer_Return_Children_For_Null()
        {
            var store = CreateStoreObject();
            var products = store.Products[null];

            Assert.AreEqual(2, products.Count);
        }

        [Description("Test to verify the indexer works nice when passing a named item"), TestMethod()]
        public void Check_The_Object_Return_Named_Children()
        {
            var store = CreateStoreObject();
            var owner = store["Owner"] as IEnumerable<dynamic>;
            Assert.AreEqual(owner.First().FirstName, "Jack");
        }


        [Description("Test to verify '<<' operator to add elements"), TestMethod()]
        public void Check_The_LeftShift_Operator_Can_Add_An_Element_By_Name()
        {
            dynamic myobj = new ElasticObject("MyObject");

            for (int i = 0; i < 10; i++)
            {
                var newItem = myobj << "Item";
                newItem.CountNumber = i;
            }

            Assert.AreEqual(10, myobj["Item"].Count);

        }

        [Description("Test to verify '<' operator to add attributes"), TestMethod()]
        public void Check_The_LessThan_Operator_Can_Add_An_Attribute_By_Name()
        {
            dynamic myobj = new ElasticObject("MyObject");

            for (int i = 0; i < 10; i++)
            {
                var newItem = myobj < "Attrib" + i;
                newItem <<= "somevalue";
            }

            //Few random checks

            Assert.AreEqual(myobj.Attrib1, "somevalue");
            Assert.AreEqual(myobj.Attrib8, "somevalue");
        }


        [Description("Test to verify integer in Indexer"), TestMethod()]
        public void Check_The_Object_Indexer_Can_Accept_Integers()
        {
            dynamic myobj = new ElasticObject("MyObject");

            for (int i = 0; i < 10; i++)
            {
                var newItem = myobj << "Item";
                newItem.CountNumber = i;
            }

            //Check the 3rd and 9th items
            Assert.AreEqual(3, myobj[3].CountNumber);
            Assert.AreEqual(9, myobj[9].CountNumber);
        }


        [Description("Test to verify '<<' and '<' operator to add elements and attributes"), TestMethod()]
        public void Check_The_Element_And_Attributes_Can_Be_Added_Using_Operators()
        {
            dynamic myobj = new ElasticObject("MyObject");

            var attrib1= (myobj << "element") < "attribute1";
            attrib1 <<= "hello1";
            var attrib2 = (myobj << "element") < "attribute1";
            attrib2 <<= "hello2";

            //first element
            Assert.AreEqual(myobj[0].attribute1, "hello1");

            //second element
            Assert.AreEqual(myobj[1].attribute1, "hello2");

        }


        [Description("Test to verify Indexer can accept a filter delegate"), TestMethod()]
        public void Check_The_Indexer_Take_A_Delegate()
        {
            dynamic myobj = new ElasticObject("MyObject");

            for (int i = 0; i < 10; i++)
            {
                var newItem = myobj << "Item";
                newItem.CountNumber = i;
            }

            var filter=new Func<dynamic,bool>((obj)=>obj.CountNumber>5);
            var result = myobj[filter];

            //4 items remains above 5
            Assert.AreEqual(4, result.Count);
        }


    }
}
