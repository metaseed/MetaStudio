using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Media;

using System.Printing;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows.Markup;
namespace Metaseed.Windows
{
    public class FrameworkElementExport
    {

        static public void ExportXaml(Uri path, FrameworkElement surface)
        {
            if (path == null) return;
            if (surface == null) return;
            string xaml = XamlWriter.Save(surface);
            File.WriteAllText(path.LocalPath, xaml);
        }

        static public void ExportXpsDoc(Uri path, FrameworkElement surface)
        {
            if (path == null) return;
            // Save current canvas transorm
            Transform transform = surface.LayoutTransform;
            // Temporarily reset the layout transform before saving
            surface.LayoutTransform = null;

            // Get the size of the canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange elements
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Open new package
            Package package = Package.Open(path.LocalPath, FileMode.Create);
            // Create new xps document based on the package opened
            XpsDocument doc = new XpsDocument(package);
            // Create an instance of XpsDocumentWriter for the document
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            // Write the canvas (as Visual) to the document
            writer.Write(surface);
            // Close document
            doc.Close();
            // Close package
            package.Close();

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }

        static public void ExportPng(Uri path, FrameworkElement surface)
        {
            if (path == null) return;
            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Bgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
        }

        static public void ExportSmallPngs(FrameworkElement frameworkElement, int size, DirectoryInfo path)
        {
            BitmapImage bitmapImage = ExportBitmapImage(frameworkElement);
            for (int startX = 0; startX < frameworkElement.Width; startX += size)
            {
                for (int startY = 0; startY < frameworkElement.Height; startY += size)
                {
                    ExportPng(bitmapImage, startX, startY, size, size, path.FullName + "\\" + startX.ToString() + "-" + startY.ToString() + ".png");
                }
            }
        }

        static public BitmapImage ExportBitmapImage(FrameworkElement frameworkElement)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)frameworkElement.ActualWidth,
                                                            (int)frameworkElement.ActualHeight,
                                                            96d,
                                                            96d,
                                                            PixelFormats.Default);
            rtb.Render(frameworkElement);

            MemoryStream stream = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        static public RenderTargetBitmap CropImage(BitmapSource sourceImage,
                               double startX,
                               double startY,
                               int width,
                               int height
                               )
        {
            TransformGroup transformGroup = new TransformGroup();
            TranslateTransform translateTransform = new TranslateTransform();
            translateTransform.X = -startX;
            translateTransform.Y = -startY;
            transformGroup.Children.Add(translateTransform);

            DrawingVisual vis = new DrawingVisual();
            DrawingContext cont = vis.RenderOpen();
            cont.PushTransform(transformGroup);
            cont.DrawImage(sourceImage, new Rect(new Size(sourceImage.PixelWidth, sourceImage.PixelHeight)));
            cont.Close();

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Default);
            rtb.Render(vis);
            return rtb;
        }

        static public void ExportPng(BitmapSource sourceImage,
                               double startX,
                               double startY,
                               int width,
                               int height,
                               string filePath)
        {
            RenderTargetBitmap rtb = CropImage(sourceImage, startX, startY, width, height);
            FileStream stream = new FileStream(filePath, FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
            stream.Close();
        }
        

        //static  void PrintVisual(Canvas element, string description, double minMargin)
        //{

        //    PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
        //    //http://denisvuyka.wordpress.com/2007/12/03/wpf-diagramming-saving-you-canvas-to-image-xps-document-or-raw-xaml/
        //    //http://www.switchonthecode.com/tutorials/csharp-tutorial-image-editing-saving-cropping-and-resizing
        //    if (printDlg.ShowDialog() == true)
        //    {
        //        Size szOrg = new Size(element.ActualWidth, element.ActualHeight);
        //        ////System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);
        //        //////get scale of the print wrt to screen of WPF visual
        //        ////double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / element.ActualWidth, capabilities.PageImageableArea.ExtentHeight /element.ActualHeight);
        //        //// //Transform the Visual to scale
        //        //// element.LayoutTransform = new ScaleTransform(scale, scale); 
        //        //// //get the size of the printer page
        //        //// Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
        //        //// //update the layout of the visual to the printer page size.
        //        //// element.Measure(sz); 
        //        //// element.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
        //        //// //now print the visual to printer to fit on the one page.
        //        //// printDlg.PrintVisual(element, "First Fit to Page WPF Print");
        //        BitmapImage bi = VisualToBitmapImage(element);
        //        //Rect bounds=element.bo
        //        ////SaveImage(bi, (int)left, (int)top, (int)(right - left), (int)(bottom - top), @"..\..\Data\aa.png");
        //        //var ima = CropImage(bi, (int)left, (int)top, (int)(right - left), (int)(bottom - top));

        //        ////get the size of the printer page 
        //PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);
        //        //double pageMargin = Math.Min(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight);
        //        //double additionalMargin = Math.Max(0, minMargin - pageMargin);
        //        //Size pageSize = new Size(capabilities.PageImageableArea.ExtentWidth - additionalMargin * 2, capabilities.PageImageableArea.ExtentHeight - additionalMargin * 2);
        //        ////get scale of the print wrt to screen of WPF visual 
        //        //double scale = Math.Min(pageSize.Width / (right - left), pageSize.Height / (bottom - top));
        //        //Size printSize = new Size((right - left) * scale, (bottom - top) * scale);
        //        //Point upperLeft = new Point(capabilities.PageImageableArea.OriginWidth + additionalMargin, capabilities.PageImageableArea.OriginHeight + additionalMargin);

        //        ////Transform the Visual to scale 

        //        //ima.LayoutTransform = new ScaleTransform(scale, scale);
        //        //ima.Measure(printSize);
        //        //ima.Arrange(new Rect(upperLeft, printSize));
        //        //printDlg.PrintVisual(ima, description);
        //        //element.LayoutTransform = new ScaleTransform(scale, scale);
        //        //element.Measure(printSize);
        //        //element.Arrange(new Rect(upperLeft, printSize));
        //        //printDlg.PrintVisual(element, description);

        //        ////Capture the image of the visual in the same size as Printing page.  
        //        //RenderTargetBitmap bmp = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
        //        //bmp.Render(element);
        //        //FixedDocument myDocument = new FixedDocument();
        //        //for (int offset = 0; offset < element.ActualHeight; offset += (int)capabilities.PageImageableArea.ExtentHeight)
        //        //{
        //        //    DrawingVisual drawingVisual = new DrawingVisual();
        //        //    //create a drawing context so that image can be rendered to print  
        //        //    DrawingContext dc = drawingVisual.RenderOpen();
        //        //    dc.PushTransform(new TranslateTransform(0, -offset));
        //        //    dc.DrawImage(bmp, new System.Windows.Rect(sz));
        //        //    dc.Close();
        //        //    //now print the image visual to printer to fit on the one page.  
        //        //    PageContent pageContent = new PageContent();
        //        //    FixedPage page = new FixedPage();
        //        //    page.Width = capabilities.PageImageableArea.ExtentWidth;
        //        //    page.Height = capabilities.PageImageableArea.ExtentHeight;
        //        //    //container holds our visual...  
        //        //    VisualContainer myContainer = new VisualContainer();
        //        //    myContainer.AddVisual(drawingVisual);
        //        //    //add container to the page  
        //        //    page.Children.Add(myContainer);
        //        //    ((IAddChild)pageContent).AddChild(page);
        //        //    //add page to document  
        //        //    myDocument.Pages.Add(pageContent);
        //        //}
        //        //printDlg.PrintDocument(myDocument.DocumentPaginator, description);
        //        //element.Measure(szOrg);
        //    }
        //}
        //static void ShowPrintPreview(FixedDocument fixedDoc)
        //{

        //    Window wnd = new Window();

        //    DocumentViewer viewer = new DocumentViewer();

        //    viewer.Document = fixedDoc;

        //    wnd.Content = viewer;

        //    wnd.ShowDialog();

        //}


    }
}
