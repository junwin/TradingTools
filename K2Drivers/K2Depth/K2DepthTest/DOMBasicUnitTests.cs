using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KaiTrade.Interfaces;

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

        public void DOMUpdate(object sender, decimal? price)
        {
        }

        [TestMethod]
        public void TestSimpleDOMUpdate()
        {
            IDOM dom = new K2Depth.K2DOM();
            Assert.IsNotNull(dom);

            IDOMData domData = dom.Create(100.00M, 20M, 1);
            Assert.IsNotNull(domData);

            dom.Update(101M, 5, 4);

            int slotIndex = dom.GetSlotIndex(101M);

            Assert.AreNotEqual(-1, slotIndex);

            IDOMSlot slot = dom.DOMData.K2DOMSlots[slotIndex];

            Assert.IsNotNull(slot);

            Assert.AreEqual(101M, slot.Price);
            Assert.AreEqual(5M, slot.BidSize);
            Assert.AreEqual(4M, slot.AskSize);
            

            // Apply simple update

            // test expected slot exists

            // Test values in the slot

            // update neprice

            // Test original slot not changed

            // Update original slot

           
        }
    }
}
