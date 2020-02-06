using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.IO;
using Drawing = DocumentFormat.OpenXml.Drawing;
using arkAS.BLL.Core;

namespace arkAS.BLL.Pptx
{
    public class Pptx
    {

        private string folder;
        public void SetFolder(string folder)
        {
            this.folder = folder;
        }
        public int CountSlide()
        {
            using (PresentationDocument ppt = PresentationDocument.Open(HttpContext.Current.Server.MapPath(folder), true))
            {
                PresentationPart part = ppt.PresentationPart;
                OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;
                ppt.Close();
                return slideIds.Count();
            }
            
        }

        public void SetPPTShapeText(string[] arrayParameters, int numSlide)
        {
            using (PresentationDocument ppt = PresentationDocument.Open(HttpContext.Current.Server.MapPath(folder), true))
            {
                numSlide--;
                IEnumerable<Shape> elementSlide;
                PresentationPart part = ppt.PresentationPart;
                OpenXmlElementList slideIds = part.Presentation.SlideIdList.ChildElements;
                string relId = (slideIds[numSlide] as SlideId).RelationshipId;

                SlidePart slide = (SlidePart)part.GetPartById(relId);
                if (slide != null)
                {
                    ShapeTree tree = slide.Slide.CommonSlideData.ShapeTree;

                    elementSlide = tree.Elements<Shape>();

                    foreach (Shape shape1 in elementSlide)
                    {
                        string valueStr = shape1.NonVisualShapeProperties.NonVisualDrawingProperties.Name.Value;
                        switch (valueStr)
                        {
                            case "Заголовок 1":
                               Drawing.Paragraph paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                               Drawing.Run run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                               run1.Text.Text = arrayParameters[0];
                               slide.Slide.Save();
                                break;
                            case "Прямоугольник 4":
                               paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                               run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                               run1.Text.Text = arrayParameters[1];
                               slide.Slide.Save();
                                break;
                            case "Прямоугольник 6":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[2];
                                slide.Slide.Save();
                                break;
                            case "Прямоугольник 9":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[3];
                                slide.Slide.Save();
                                break;
                            case "Прямоугольник 11":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[4];
                                slide.Slide.Save();
                                break;
                            case "Прямоугольник 13":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[5];
                                slide.Slide.Save();
                                break;
                            case "Прямоугольник 18":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[6];
                                slide.Slide.Save();
                                break;
                            case "Прямоугольник 20":
                                paragraph1 = shape1.TextBody.ChildElements.OfType<Drawing.Paragraph>().ElementAt(0);
                                run1 = paragraph1.ChildElements.OfType<Drawing.Run>().ElementAt(0);
                                run1.Text.Text = arrayParameters[7];
                                slide.Slide.Save();
                                break;
                        }

                        }

                    }
                ppt.Close();
                }
            }


        public bool CloneSlidePart(int i)
        {
            using (PresentationDocument ppt = PresentationDocument.Open(HttpContext.Current.Server.MapPath(folder), true))
            {
                PresentationPart presentationPart = ppt.PresentationPart;
                OpenXmlElementList slideIds = presentationPart.Presentation.SlideIdList.ChildElements;
                string relId = (slideIds[0] as SlideId).RelationshipId;

                //
                uint max = (slideIds[0] as SlideId).Id;
                uint k=0;
                for (int j = 0; j < i; j++)
                {
                     k = (slideIds[j] as SlideId).Id;
                     if (k > max)
                         max = k;

                }
                
                SlidePart slideTemplate = (SlidePart)presentationPart.GetPartById(relId);
                SlidePart newSlidePart = presentationPart.AddNewPart<SlidePart>("newSlide" + max);
                newSlidePart.FeedData(slideTemplate.GetStream(FileMode.Open));
                newSlidePart.AddPart(slideTemplate.SlideLayoutPart);
                SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;
                uint maxSlideId = 1;
                SlideId prevSlideId = null;
                foreach (SlideId slideId in slideIdList.ChildElements)
                {
                    if (slideId.Id > maxSlideId)
                    {
                        maxSlideId = slideId.Id;
                        prevSlideId = slideId;
                    }
                }
                maxSlideId++;
                SlideId newSlideId = slideIdList.InsertAfter(new SlideId(), prevSlideId);
                newSlideId.Id = maxSlideId;
                newSlideId.RelationshipId = presentationPart.GetIdOfPart(newSlidePart);

                if (newSlidePart != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public  bool DeleteSlide(int slideIndex, int slidesCount)
        {
            if (slidesCount == 1)
                return false;

            using (PresentationDocument presentationDocument = PresentationDocument.Open(HttpContext.Current.Server.MapPath(folder), true))
            {
                DeleteSlide(presentationDocument, slideIndex, slidesCount);
                return true;
            }
        }  



        public static void DeleteSlide(PresentationDocument presentationDocument, int slideIndex, int slidesCount)
        {
            slideIndex--;
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

          

            if (slideIndex < 0 || slideIndex >= slidesCount)
            {
                throw new ArgumentOutOfRangeException("slideIndex");
            }
 
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            Presentation presentation = presentationPart.Presentation;

            SlideIdList slideIdList = presentation.SlideIdList;

            SlideId slideId = slideIdList.ChildElements[slideIndex] as SlideId;

            string slideRelId = slideId.RelationshipId;

            slideIdList.RemoveChild(slideId);

            if (presentation.CustomShowList != null)
            {
                foreach (var customShow in presentation.CustomShowList.Elements<CustomShow>())
                {
                    if (customShow.SlideList != null)
                    {
                        LinkedList<SlideListEntry> slideListEntries = new LinkedList<SlideListEntry>();
                        foreach (SlideListEntry slideListEntry in customShow.SlideList.Elements())
                        {
                            if (slideListEntry.Id != null && slideListEntry.Id == slideRelId)
                            {
                                slideListEntries.AddLast(slideListEntry);
                            }
                        }

                        foreach (SlideListEntry slideListEntry in slideListEntries)
                        {
                            customShow.SlideList.RemoveChild(slideListEntry);
                        }
                    }
                }
            }

            presentation.Save();

            SlidePart slidePart = presentationPart.GetPartById(slideRelId) as SlidePart;

            presentationPart.DeletePart(slidePart);
        }




    }
}