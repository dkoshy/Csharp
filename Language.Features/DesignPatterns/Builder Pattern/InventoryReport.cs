using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Builder_Pattern
{
    public class FurnitureItem
    {
        public string Name;
        public double Price;
        public double Height;
        public double Width;
        public double Weight;

        public FurnitureItem(string productName, double price, double height, double width, double weight)
        {
            this.Name = productName;
            this.Price = price;
            this.Height = height;
            this.Width = width;
            this.Weight = weight;
        }
    }

    public class InventoryReport
    {
        public string TitleSection;
        public string DimensionsSection;
        public string LogisticsSection;

        public string Debug()
        {
            return new StringBuilder()
                .AppendLine(TitleSection)
                .AppendLine(DimensionsSection)
                .AppendLine(LogisticsSection)
                .ToString();
        }
    }

    public interface IInventoryReportBuilder
    {
        IInventoryReportBuilder AddTitle();
        IInventoryReportBuilder AddDimentions();
        IInventoryReportBuilder AddLogistics(DateTime dateTime);

        InventoryReport GetReport();

    }

    public class DailyReportBuilder : IInventoryReportBuilder
    {
        private readonly IEnumerable<FurnitureItem> _furnitureItems;
        private InventoryReport _report;

        public DailyReportBuilder(IEnumerable<FurnitureItem>  furnitureItems)
        {
            Reset();
            _furnitureItems = furnitureItems;
        }
        public IInventoryReportBuilder AddDimentions()
        {
            var dimensions = string.Join(Environment.NewLine, _furnitureItems.Select(f =>
                     $"ProductName :  {f.Name}, Price :{f.Price} {Environment.NewLine}"
                      + $"Width : {f.Width} X Height : {f.Height} X Weight : {f.Height}"));
            _report.DimensionsSection = dimensions;
            return this;
        }

        public IInventoryReportBuilder AddLogistics(DateTime dateTime)
        {
            _report.LogisticsSection = $"Report generated on {dateTime}";
            return this;
        }

        public IInventoryReportBuilder AddTitle()
        {

            var title = $"-----Daily Report------{Environment.NewLine}";
            _report.TitleSection = title;

            return this;
        }

        public InventoryReport GetReport()
        {
            var Dailyreport = _report;
            Reset();
            return Dailyreport;
        }


        private void Reset()
        {
            _report = new InventoryReport();
        }
    }

    public class InventoryReportBuildDirector
    {
        private readonly IInventoryReportBuilder _reportBuilder;

        public InventoryReportBuildDirector(IInventoryReportBuilder reportBuilder)
        {
            _reportBuilder = reportBuilder;
        }

        public void  BuildReport()
        {
            _reportBuilder.AddTitle()
                .AddDimentions()
                .AddLogistics(DateTime.Now);
               
        }
    }
}
