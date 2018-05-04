PREREQUSITES:
1. Unity -> VolumeViewerPro
2. VRTK
3. Various code files from previous semester(s).

  INSTALLING VOLUMEVIWERPRO
  1. If necessary, buy and download from https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/volume-viewer-pro-83185
          a. Otherwise,use as included with project.
  2. Consult LISCINTEC VolumeViewerPro Pdf file for further instructions, including downloading, installation, etc.
  
  LOADING DICOM IMAGES
  1. Consult VolumeViewerPro SECTION 5 (page 8 of Pdf file)

  DICOM MENU
  1. To modify DICOM menu attributes:
    1a. To modify the DICOM menu panel code, check the DICOMMenu.Cs file (code for buttons and sliders here as well)
    1b. To change transfer function files: [VRTK_Scripts] -> PlayArea -> DICOM Menu -> Tf Data Mode (Script).
        i. Increase size
        ii. Go to Element
        iii. Select small button on far right of element
        iv. Choose texture: a 256x256 image of the transfer function (done in Gimp for our project but alternate sources are possible)
        v. To change layout of the transfer functions on the display screen, scroll up to Rect Transform and select each button to change layout.
        vi. To actually add new buttons to DICOM Menu display, Scroll up/down to Dicom Menu (Script), and select button in the space. Check above to 1a where to actually add code. 
	ANGLE VIWER
	1. As this is solely modified and carried out within the code, check code comments to understand more.
  
ACKNOWLEDGEMENTS
  A. ASSETS
    I. UNITY VolumeViewerPro
    II. VRTK
  B. TEAMS
    I. UTD Fall 2017 UTSW Team
            1. Brandon Marzik
            2. Catherine Nguyen
            3. Gabriela Rodriguez
            4. Husam Abdelhadi
    II. UTD Spring 2018 UTSW Team - DICOM Import and Visualization of C. Heart Diseases in VR
            1. William Ben Speicher
            2. Lane Miller
            3. Venkata Ponakala
            4. Alexander Page
            5. Taylor Rowland