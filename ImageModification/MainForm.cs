﻿/*
 * The Following Code was developed by Dewald Esterhuizen
 * View Documentation at: http://softwarebydefault.com
 * Licensed under Ms-PL 
 * It was further modified by Zappellaz Nancy & Mabillard Julien
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace ImageEdgeDetection
{
    public partial class MainForm : Form
    {

        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        //Variable previewModifiedFilter used to store the image preview after its modification by a filter
        private Bitmap previewModifiedFilter = null;
        //Variable resultFilterBitmap used to store the original image after its modification by a filter
        private Bitmap resultFilterBitmap = null;
        //Image to be saved at the end of the process
        private Bitmap resultBitmap = null;
        //Boolean used to distinguish if the user is applying edge filters (edge = true) or color filters, used by the ValueChangedEventHandler
        private bool edge = false;
        //Variable used for the custom filter 
        private Color customColor;
        //Boolean used to not open dialogColor on click on btnGoBack and btnEdgeDetection
        private bool isOnBtnClick = false;

        public MainForm()
        {
            InitializeComponent();

            cmbApplyFilter.SelectedIndex = 0;

            // Elements that appear only for the edge detection are made invisible
            cmbEdgeDetection.Visible = false;
            cmbApplyFilter.Visible = false;
            btnSaveNewImage.Visible = false;
            btnGoBack.Visible = false;
            btnApplyEdgeDetection.Visible = false;


        }

        //Button for loading image 
        private void btnOpenOriginal_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select an image file.";
            ofd.Filter = "Png Images(*.png)|*.png|Jpeg Images(*.jpg)|*.jpg";
            ofd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(ofd.FileName);
                originalBitmap = (Bitmap)Bitmap.FromStream(streamReader.BaseStream);
                streamReader.Close();

                previewBitmap = originalBitmap.CopyToSquareCanvas(picPreview.Width);
                picPreview.Image = previewBitmap;


                ApplyFilter(true);

                //Since there is an image, it is possible to filter it and to go to the edge detection, so the corresponding buttons appear
                cmbApplyFilter.Visible = true;
                btnApplyEdgeDetection.Visible = true;
            }
        }

        //Button to go to the edge detection
        private void btnApplyEdgeDetection_Click(object sender, EventArgs e)
        {

            //For the ValueChangedEventHandler
            edge = true;

            //Filter applied is saved and passed further
            isOnBtnClick = true;
            ApplyFilter(false);

            //We change the elements usable by making them visible or invisible
            cmbApplyFilter.Visible = false;
            btnApplyEdgeDetection.Visible = false;
            btnOpenOriginal.Visible = false;
            btnGoBack.Visible = true;
            btnSaveNewImage.Visible = true;
            cmbEdgeDetection.Visible = true;
            cmbEdgeDetection.SelectedIndex = 0;


        }

        //Button to go back to filters from edge detection
        private void btnGoBack_Click(object sender, EventArgs e)
        {
            //We show the preview of the modified image by a filter
            picPreview.Image = previewModifiedFilter;

            //We change the elements usable by making them visible or invisible         
            isOnBtnClick = false;
            cmbEdgeDetection.Visible = false;
            btnGoBack.Visible = false;
            btnSaveNewImage.Visible = false;
            cmbApplyFilter.Visible = true;
            btnApplyEdgeDetection.Visible = true;
            btnOpenOriginal.Visible = true;

            //For the ValueChangedEventHandler
            edge = false;
        }

        //Button for saving image (original method)
        private void btnSaveNewImage_Click(object sender, EventArgs e)
        {
            ApplyEdgeDetection(false);

            if (resultBitmap != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Specify a file name and file path";
                sfd.Filter = "Png Images(*.png)|*.png|Jpeg Images(*.jpg)|*.jpg";
                sfd.Filter += "|Bitmap Images(*.bmp)|*.bmp";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileExtension = Path.GetExtension(sfd.FileName).ToUpper();
                    ImageFormat imgFormat = ImageFormat.Png;

                    if (fileExtension == "BMP")
                    {
                        imgFormat = ImageFormat.Bmp;
                    }
                    else if (fileExtension == "JPG")
                    {
                        imgFormat = ImageFormat.Jpeg;
                    }

                    StreamWriter streamWriter = new StreamWriter(sfd.FileName, false);
                    //Line added to avoid an error : http://stackoverflow.com/questions/15571022/how-to-find-reason-for-generic-gdi-error-when-saving-an-image
                    Bitmap savedImage = new Bitmap(resultBitmap);
                    savedImage.Save(streamWriter.BaseStream, imgFormat);
                    streamWriter.Flush();
                    streamWriter.Close();

                    resultBitmap = null;
                }
            }
        }

        //First part dedicated to the filters
        private void ApplyFilter(bool preview)
        {

            if (previewBitmap == null || cmbApplyFilter.SelectedIndex == -1)
            {
                return;
            }

            Bitmap imageToFilter = null;
            Bitmap bitmapResultFilter = null;

            //If the image is previewed we work on the preview image
            if (preview == true)
            {
                imageToFilter = previewBitmap;
            }
            //Else we work on the original image 
            else
            {
                imageToFilter = originalBitmap;
            }

            //The filter to apply is selected from the dropdownlist
            String filterSelected = cmbApplyFilter.SelectedItem.ToString();


            switch (filterSelected)
            {
                case "No filter chosen":
                    bitmapResultFilter = imageToFilter;
                    break;

                case "Night Filter":
                    bitmapResultFilter = imageToFilter.NightFilter();
                    break;

                case "Hell Filter":
                    bitmapResultFilter = imageToFilter.HellFilter();
                    break;

                case "Miami Filter":
                    bitmapResultFilter = imageToFilter.MiamiFilter();
                    break;

                case "Zen Filter":
                    bitmapResultFilter = imageToFilter.ZenFilter();
                    break;

                case "Black and White":
                    bitmapResultFilter = imageToFilter.BlackNWhite();
                    break;

                case "Swap Filter":
                    bitmapResultFilter = imageToFilter.SwapFilter();
                    break;

                case "Crazy Filter":
                    bitmapResultFilter = imageToFilter.CrazyFilter();
                    break;

                case "Mega Filter Green":
                    bitmapResultFilter = imageToFilter.MegaFilterGreen();
                    break;


                case "Mega Filter Orange":
                    bitmapResultFilter = imageToFilter.MegaFilterOrange();
                    break;

                case "Mega Filter Pink":
                    bitmapResultFilter = imageToFilter.MegaFilterPink();
                    break;

                case "Mega Filter Custom":
                    //Test to not open the colorDialog on click on EdgeDetection button or GoBack button
                    if (isOnBtnClick == false)
                    {
                        /*If OpenColorDialog is true, it means that the cancel button was clicked, 
                         *so no change is made and the dropdownlist is reset 
                         */
                        if (OpenColorDialog() == true)
                        {
                            cmbApplyFilter.SelectedIndex = 0;
                            return;
                        }
                    }

                    bitmapResultFilter = imageToFilter.MegaFilterCustom(customColor);

                    break;

                case "Rainbow Filter":
                    bitmapResultFilter = imageToFilter.Rainbow();
                    break;
            }



            if (bitmapResultFilter != null)
            {
                //If it is a preview the result is shown in the application
                if (preview == true)
                {
                    picPreview.Image = bitmapResultFilter;
                }
                //If not, it means that it will be passed to the edge detection
                else
                {
                    resultFilterBitmap = bitmapResultFilter;
                    //Used to store the preview in the next phase (edge detection), to avoid loss of preview quality
                    previewModifiedFilter = (Bitmap)picPreview.Image;
                }
            }
        }

        //Open the Dialog to choose a color and check if the cancel button was clicked
        public bool OpenColorDialog()
        {
            ColorDialog CD = new ColorDialog();

            //Check if the cancel button was clicked
            bool cancel = true;

            if (CD.ShowDialog() == DialogResult.OK)
            {
                Color newC = CD.Color;
                customColor = newC;
                cancel = false;
            }
            return cancel;
        }

        //Application of edge detection (mainly original method)
        private void ApplyEdgeDetection(bool preview)
        {
            if (cmbEdgeDetection.SelectedIndex == -1)
            {
                return;
            }

            Bitmap imageForEdgeDetection = null;
            Bitmap bitmapResultEdge = null;

            if (preview == true)
            {
                //If it is for the preview, we work on the preview image
                imageForEdgeDetection = previewModifiedFilter;
            }
            else
            {
                //If the image is going to be saved, it is the original image filtered that is modified
                imageForEdgeDetection = resultFilterBitmap;
            }


            //The filter to apply is selected from the dropdownlist
            String edgeDetectionSelected = cmbEdgeDetection.SelectedItem.ToString();

            switch (edgeDetectionSelected)
            {
                case "No edge detection chosen":
                    bitmapResultEdge = imageForEdgeDetection;
                    break;

                case "Laplacian 3x3":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian3x3Filter(false);
                    break;

                case "Laplacian 3x3 Grayscale":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian3x3Filter(true);
                    break;

                case "Laplacian 5x5":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian5x5Filter(false);
                    break;

                case "Laplacian 5x5 Grayscale":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian5x5Filter(true);
                    break;

                case "Laplacian of Gaussian":
                    bitmapResultEdge = imageForEdgeDetection.LaplacianOfGaussianFilter();
                    break;

                case "Laplacian 3x3 of Gaussian 3x3":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian3x3OfGaussian3x3Filter();
                    break;

                case "Laplacian 3x3 of Gaussian 5x5 - 1":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian3x3OfGaussian5x5Filter1();
                    break;

                case "Laplacian 3x3 of Gaussian 5x5 - 2":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian3x3OfGaussian5x5Filter2();
                    break;

                case "Laplacian 5x5 of Gaussian 3x3":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian5x5OfGaussian3x3Filter();
                    break;

                case "Laplacian 5x5 of Gaussian 5x5 - 1":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian5x5OfGaussian5x5Filter1();
                    break;

                case "Laplacian 5x5 of Gaussian 5x5 - 2":
                    bitmapResultEdge = imageForEdgeDetection.Laplacian5x5OfGaussian5x5Filter2();
                    break;

                case "Sobel 3x3":
                    bitmapResultEdge = imageForEdgeDetection.Sobel3x3Filter(false);
                    break;

                case "Sobel 3x3 Grayscale":
                    bitmapResultEdge = imageForEdgeDetection.Sobel3x3Filter();
                    break;

                case "Prewitt":
                    bitmapResultEdge = imageForEdgeDetection.PrewittFilter(false);
                    break;

                case "Prewitt Grayscale":
                    bitmapResultEdge = imageForEdgeDetection.PrewittFilter();
                    break;

                case "Kirsch":
                    bitmapResultEdge = imageForEdgeDetection.KirschFilter(false);
                    break;

                case "Kirsch Grayscale":
                    bitmapResultEdge = imageForEdgeDetection.KirschFilter();
                    break;
            }

            if (bitmapResultEdge != null)
            {
                //If it is a preview the result is shown in the application
                if (preview == true)
                {
                    picPreview.Image = bitmapResultEdge;
                }
                //If not, it means that it will be saved as the final result
                else
                {
                    resultBitmap = bitmapResultEdge;

                }
            }
        }



        private void NeighbourCountValueChangedEventHandler(object sender, EventArgs e)
        {
            //The two parts of the application are distinguished by the boolean edge that determines if it is EdgeDetection or Filter that is activated
            if (edge == true)
            {
                ApplyEdgeDetection(true);
            }
            else
            {
                ApplyFilter(true);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
