using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KaiTrade.Interfaces;
using System.Threading;

namespace K2DepthTest
{
    [TestClass]
    public class DOMBasicUnitTests
    {
        [TestMethod]
        public void CreateDOMInstance()
        {
            IDOM dom = new K2Depth.K2DOM();
            Assert.IsNotNull(dom);

            // test that the slot is not found for an aribitary value
            // since no slots have been created
            Assert.AreEqual(-1, dom.GetSlotIndex(999123.00M));
        }

        [TestMethod]
        public void CreateDOMData()
        {
            IDOM dom = new K2Depth.K2DOM();
            Assert.IsNotNull(dom);

            IDOMData domData =  dom.Create(100.00M, 20M, 1);
            Assert.IsNotNull(domData);

            Assert.AreEqual(41, domData.MaxSlots);
            Assert.AreEqual(80, domData.MinPrice);
            Assert.AreEqual(120, domData.MaxPrice);

            // test that the slot is not found for an aribitary value
            // 
            Assert.AreEqual(-1, dom.GetSlotIndex(999123.00M));

            // test we get the correct index for init price
            Assert.AreEqual(20, dom.GetSlotIndex(100.00M));
            Assert.AreEqual(0, dom.GetSlotIndex(80.00M));
            Assert.AreEqual(40, dom.GetSlotIndex(120.00M));
            

        }

        public void DOMImage(object sender)
        {
        }

        List<IDOMSlot> lastUpdatedSots = null;

        public void DOMUpdate(object sender, List<IDOMSlot> updatedSlots)
        {
            lastUpdatedSots = updatedSlots;
        }

        [TestMethod]
        public void TestSimpleDOMUpdate()
        {
            lastUpdatedSots = null;

            IDOM dom = new K2Depth.K2DOM();
            Assert.IsNotNull(dom);

            // wire up the handlers
            dom.DOMImage += DOMImage;
            dom.DOMUpdate += DOMUpdate;

            IDOMData domData = dom.Create(100.00M, 20M, 1);
            Assert.IsNotNull(domData);

            lastUpdatedSots = new List<IDOMSlot>();

            dom.Update(101M, 5, 4);

            System.Threading.Thread.Sleep(1000);

            int slotIndex = dom.GetSlotIndex(101M);

            Assert.AreNotEqual(-1, slotIndex);

            IDOMSlot slot = dom.DOMData.K2DOMSlots[slotIndex];

            Assert.IsNotNull(slot);

            Assert.AreEqual(101M, slot.Price);
            Assert.AreEqual(5M, slot.BidSize);
            Assert.AreEqual(4M, slot.AskSize);

            Assert.IsNotNull(lastUpdatedSots);
            Assert.AreEqual(1, lastUpdatedSots.Count);
            Assert.AreEqual(101M, lastUpdatedSots[0].Price);
            Assert.AreEqual(5M, lastUpdatedSots[0].BidSize.Value);
            Assert.AreEqual(4M, lastUpdatedSots[0].AskSize.Value);

            lastUpdatedSots = new List<IDOMSlot>();

            dom.Update(99M, 3, 5);
            System.Threading.Thread.Sleep(1000);
            
            // make sure 101 is not affected
            slotIndex = dom.GetSlotIndex(101M);

            Assert.AreNotEqual(-1, slotIndex);

            slot = dom.DOMData.K2DOMSlots[slotIndex];

            Assert.IsNotNull(slot);

            Assert.AreEqual(101M, slot.Price);
            Assert.AreEqual(5M, slot.BidSize);
            Assert.AreEqual(4M, slot.AskSize);

            // make sure 99 is updated
            slotIndex = dom.GetSlotIndex(99M);

            Assert.AreNotEqual(-1, slotIndex);

            slot = dom.DOMData.K2DOMSlots[slotIndex];

            Assert.IsNotNull(slot);

            Assert.AreEqual(99M, slot.Price);
            Assert.AreEqual(3M, slot.BidSize);
            Assert.AreEqual(5M, slot.AskSize);


            // Update existing
            lastUpdatedSots = new List<IDOMSlot>();
            dom.Update(99M, 101, 5);
            System.Threading.Thread.Sleep(1000);
            // make sure 99 is updated
            slotIndex = dom.GetSlotIndex(99M);

            Assert.AreNotEqual(-1, slotIndex);

            slot = dom.DOMData.K2DOMSlots[slotIndex];

            Assert.IsNotNull(slot);

            Assert.AreEqual(99M, slot.Price);
            Assert.AreEqual(101M, slot.BidSize);
            Assert.AreEqual(5M, slot.AskSize);

            Assert.AreEqual(99M, lastUpdatedSots[0].Price);
            Assert.AreEqual(101M, lastUpdatedSots[0].BidSize);
            Assert.AreEqual(5M, lastUpdatedSots[0].AskSize);



            // Apply simple update

          
            

            
           
        }
    }
}
