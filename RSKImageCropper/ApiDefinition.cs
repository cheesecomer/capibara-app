using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace RSKImageCropper
{

    ////[Verify(ConstantsInterfaceAssociation)]
    //[Static]
    //partial interface Constants
    //{
    //    // extern double RSKImageCropperVersionNumber;
    //    [Field("RSKImageCropperVersionNumber", "__Internal")]
    //    double RSKImageCropperVersionNumber { get; }

    //    // extern const unsigned char [] RSKImageCropperVersionString;
    //    [Field("RSKImageCropperVersionString", "__Internal")]
    //    byte[] RSKImageCropperVersionString { get; }

    //    // extern const CGPoint RSKPointNull;
    //    [Field("RSKPointNull", "__Internal")]
    //    CGPoint RSKPointNull { get; }
    //}

    // @interface RSKImageCropViewController : UIViewController
    [BaseType(typeof(UIViewController), Delegates = new string[] { "Delegate" }, Events = new Type[] { typeof(RSKImageCropViewControllerDelegate) })]
    interface RSKImageCropViewController
    {
        // -(instancetype _Nonnull)initWithImage:(UIImage * _Nonnull)originalImage;
        [Export("initWithImage:")]
        IntPtr Constructor(UIImage originalImage);

        // -(instancetype _Nonnull)initWithImage:(UIImage * _Nonnull)originalImage cropMode:(RSKImageCropMode)cropMode;
        [Export("initWithImage:cropMode:")]
        IntPtr Constructor(UIImage originalImage, RSKImageCropMode cropMode);

        [Wrap("WeakDelegate")]
        [NullAllowed]
        RSKImageCropViewControllerDelegate Delegate { get; set; }

        // @property (nonatomic, weak) id<RSKImageCropViewControllerDelegate> _Nullable delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        // @property (nonatomic, weak) id<RSKImageCropViewControllerDataSource> _Nullable dataSource;
        [NullAllowed, Export("dataSource", ArgumentSemantic.Weak)]
        RSKImageCropViewControllerDataSource DataSource { get; set; }

        // @property (nonatomic, strong) UIImage * _Nonnull originalImage;
        [Export("originalImage", ArgumentSemantic.Strong)]
        UIImage OriginalImage { get; set; }

        // @property (copy, nonatomic) UIColor * _Nonnull maskLayerColor;
        [Export("maskLayerColor", ArgumentSemantic.Copy)]
        UIColor MaskLayerColor { get; set; }

        // @property (assign, nonatomic) CGFloat maskLayerLineWidth;
        [Export("maskLayerLineWidth")]
        nfloat MaskLayerLineWidth { get; set; }

        // @property (copy, nonatomic) UIColor * _Nullable maskLayerStrokeColor;
        [NullAllowed, Export("maskLayerStrokeColor", ArgumentSemantic.Copy)]
        UIColor MaskLayerStrokeColor { get; set; }

        // @property (readonly, assign, nonatomic) CGRect maskRect;
        [Export("maskRect", ArgumentSemantic.Assign)]
        CGRect MaskRect { get; }

        // @property (readonly, copy, nonatomic) UIBezierPath * _Nonnull maskPath;
        [Export("maskPath", ArgumentSemantic.Copy)]
        UIBezierPath MaskPath { get; }

        // @property (assign, nonatomic) RSKImageCropMode cropMode;
        [Export("cropMode", ArgumentSemantic.Assign)]
        RSKImageCropMode CropMode { get; set; }

        // @property (readonly, nonatomic) CGRect cropRect;
        [Export("cropRect")]
        CGRect CropRect { get; }

        // @property (readonly, nonatomic) CGFloat rotationAngle;
        [Export("rotationAngle")]
        nfloat RotationAngle { get; }

        // @property (readonly, nonatomic) CGFloat zoomScale;
        [Export("zoomScale")]
        nfloat ZoomScale { get; }

        // @property (assign, nonatomic) BOOL avoidEmptySpaceAroundImage;
        [Export("avoidEmptySpaceAroundImage")]
        bool AvoidEmptySpaceAroundImage { get; set; }

        // @property (assign, nonatomic) BOOL alwaysBounceHorizontal;
        [Export("alwaysBounceHorizontal")]
        bool AlwaysBounceHorizontal { get; set; }

        // @property (assign, nonatomic) BOOL alwaysBounceVertical;
        [Export("alwaysBounceVertical")]
        bool AlwaysBounceVertical { get; set; }

        // @property (assign, nonatomic) BOOL applyMaskToCroppedImage;
        [Export("applyMaskToCroppedImage")]
        bool ApplyMaskToCroppedImage { get; set; }

        // @property (getter = isRotationEnabled, assign, nonatomic) BOOL rotationEnabled;
        [Export("rotationEnabled")]
        bool RotationEnabled { [Bind("isRotationEnabled")] get; set; }

        // @property (readonly, nonatomic, strong) UILabel * _Nonnull moveAndScaleLabel;
        [Export("moveAndScaleLabel", ArgumentSemantic.Strong)]
        UILabel MoveAndScaleLabel { get; }

        // @property (readonly, nonatomic, strong) UIButton * _Nonnull cancelButton;
        [Export("cancelButton", ArgumentSemantic.Strong)]
        UIButton CancelButton { get; }

        // @property (readonly, nonatomic, strong) UIButton * _Nonnull chooseButton;
        [Export("chooseButton", ArgumentSemantic.Strong)]
        UIButton ChooseButton { get; }

        // -(BOOL)isPortraitInterfaceOrientation;
        [Export("isPortraitInterfaceOrientation")]
        //[Verify(MethodToProperty)]
        bool IsPortraitInterfaceOrientation { get; }

        // @property (assign, nonatomic) CGFloat portraitCircleMaskRectInnerEdgeInset;
        [Export("portraitCircleMaskRectInnerEdgeInset")]
        nfloat PortraitCircleMaskRectInnerEdgeInset { get; set; }

        // @property (assign, nonatomic) CGFloat portraitSquareMaskRectInnerEdgeInset;
        [Export("portraitSquareMaskRectInnerEdgeInset")]
        nfloat PortraitSquareMaskRectInnerEdgeInset { get; set; }

        // @property (assign, nonatomic) CGFloat portraitMoveAndScaleLabelTopAndCropViewTopVerticalSpace;
        [Export("portraitMoveAndScaleLabelTopAndCropViewTopVerticalSpace")]
        nfloat PortraitMoveAndScaleLabelTopAndCropViewTopVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat portraitCropViewBottomAndCancelButtonBottomVerticalSpace;
        [Export("portraitCropViewBottomAndCancelButtonBottomVerticalSpace")]
        nfloat PortraitCropViewBottomAndCancelButtonBottomVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat portraitCropViewBottomAndChooseButtonBottomVerticalSpace;
        [Export("portraitCropViewBottomAndChooseButtonBottomVerticalSpace")]
        nfloat PortraitCropViewBottomAndChooseButtonBottomVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat portraitCancelButtonLeadingAndCropViewLeadingHorizontalSpace;
        [Export("portraitCancelButtonLeadingAndCropViewLeadingHorizontalSpace")]
        nfloat PortraitCancelButtonLeadingAndCropViewLeadingHorizontalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat portraitCropViewTrailingAndChooseButtonTrailingHorizontalSpace;
        [Export("portraitCropViewTrailingAndChooseButtonTrailingHorizontalSpace")]
        nfloat PortraitCropViewTrailingAndChooseButtonTrailingHorizontalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeCircleMaskRectInnerEdgeInset;
        [Export("landscapeCircleMaskRectInnerEdgeInset")]
        nfloat LandscapeCircleMaskRectInnerEdgeInset { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeSquareMaskRectInnerEdgeInset;
        [Export("landscapeSquareMaskRectInnerEdgeInset")]
        nfloat LandscapeSquareMaskRectInnerEdgeInset { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeMoveAndScaleLabelTopAndCropViewTopVerticalSpace;
        [Export("landscapeMoveAndScaleLabelTopAndCropViewTopVerticalSpace")]
        nfloat LandscapeMoveAndScaleLabelTopAndCropViewTopVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeCropViewBottomAndCancelButtonBottomVerticalSpace;
        [Export("landscapeCropViewBottomAndCancelButtonBottomVerticalSpace")]
        nfloat LandscapeCropViewBottomAndCancelButtonBottomVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeCropViewBottomAndChooseButtonBottomVerticalSpace;
        [Export("landscapeCropViewBottomAndChooseButtonBottomVerticalSpace")]
        nfloat LandscapeCropViewBottomAndChooseButtonBottomVerticalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeCancelButtonLeadingAndCropViewLeadingHorizontalSpace;
        [Export("landscapeCancelButtonLeadingAndCropViewLeadingHorizontalSpace")]
        nfloat LandscapeCancelButtonLeadingAndCropViewLeadingHorizontalSpace { get; set; }

        // @property (assign, nonatomic) CGFloat landscapeCropViewTrailingAndChooseButtonTrailingHorizontalSpace;
        [Export("landscapeCropViewTrailingAndChooseButtonTrailingHorizontalSpace")]
        nfloat LandscapeCropViewTrailingAndChooseButtonTrailingHorizontalSpace { get; set; }
    }

    // @protocol RSKImageCropViewControllerDataSource <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface RSKImageCropViewControllerDataSource
    {
        // @required -(CGRect)imageCropViewControllerCustomMaskRect:(RSKImageCropViewController * _Nonnull)controller;
        [Abstract]
        [Export("imageCropViewControllerCustomMaskRect:")]
        CGRect ImageCropViewControllerCustomMaskRect(RSKImageCropViewController controller);

        // @required -(UIBezierPath * _Nonnull)imageCropViewControllerCustomMaskPath:(RSKImageCropViewController * _Nonnull)controller;
        [Abstract]
        [Export("imageCropViewControllerCustomMaskPath:")]
        UIBezierPath ImageCropViewControllerCustomMaskPath(RSKImageCropViewController controller);

        // @required -(CGRect)imageCropViewControllerCustomMovementRect:(RSKImageCropViewController * _Nonnull)controller;
        [Abstract]
        [Export("imageCropViewControllerCustomMovementRect:")]
        CGRect ImageCropViewControllerCustomMovementRect(RSKImageCropViewController controller);
    }

    // @protocol RSKImageCropViewControllerDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface RSKImageCropViewControllerDelegate
    {
        // @required -(void)imageCropViewControllerDidCancelCrop:(RSKImageCropViewController * _Nonnull)controller;
        [Abstract]
        [Export("imageCropViewControllerDidCancelCrop:")]
        [EventArgs("ImageCropViewControllerCanceled")]
        [EventName("Canceled")]
        void DidCancelCrop(RSKImageCropViewController sender);

        // @required -(void)imageCropViewController:(RSKImageCropViewController * _Nonnull)controller didCropImage:(UIImage * _Nonnull)croppedImage usingCropRect:(CGRect)cropRect rotationAngle:(CGFloat)rotationAngle;
        [Abstract]
        [Export("imageCropViewController:didCropImage:usingCropRect:rotationAngle:")]
        [EventArgs("ImageCropViewControllerCropped")]
        [EventName("Cropped")]
        void DidCropImage(RSKImageCropViewController controller, UIImage croppedImage, CGRect cropRect, nfloat rotationAngle);

        // @optional -(void)imageCropViewController:(RSKImageCropViewController * _Nonnull)controller willCropImage:(UIImage * _Nonnull)originalImage;
        [Export("imageCropViewController:willCropImage:")]
        [EventArgs("ImageCropViewControllerBeganCrop")]
        [EventName("BeganCrop")]
        void WillCropImage(RSKImageCropViewController sender, UIImage originalImage);
    }

    // @interface RSKImageCropViewControllerProtectedMethods (RSKImageCropViewController)
    [Category]
    [BaseType(typeof(RSKImageCropViewController))]
    interface RSKImageCropViewController_RSKImageCropViewControllerProtectedMethods
    {
        // -(void)cropImage;
        [Export("cropImage")]
        void CropImage();

        // -(void)cancelCrop;
        [Export("cancelCrop")]
        void CancelCrop();

        // -(void)reset:(BOOL)animated;
        [Export("reset:")]
        void Reset(bool animated);

        // -(void)setRotationAngle:(CGFloat)rotationAngle;
        [Export("setRotationAngle:")]
        void SetRotationAngle(nfloat rotationAngle);

        // -(void)setZoomScale:(CGFloat)zoomScale;
        [Export("setZoomScale:")]
        void SetZoomScale(nfloat zoomScale);
    }

    // @interface RSKImageScrollView : UIScrollView
    [BaseType(typeof(UIScrollView))]
    interface RSKImageScrollView
    {
        // @property (nonatomic, strong) UIImageView * _Nullable zoomView;
        [NullAllowed, Export("zoomView", ArgumentSemantic.Strong)]
        UIImageView ZoomView { get; set; }

        // @property (assign, nonatomic) BOOL aspectFill;
        [Export("aspectFill")]
        bool AspectFill { get; set; }

        // -(void)displayImage:(UIImage * _Nonnull)image;
        [Export("displayImage:")]
        void DisplayImage(UIImage image);
    }

    // @interface RSKInternalUtility : NSObject
    [BaseType(typeof(NSObject))]
    interface RSKInternalUtility
    {
        // +(NSBundle *)bundleForStrings;
        [Static]
        [Export("bundleForStrings")]
        //[Verify(MethodToProperty)]
        NSBundle BundleForStrings { get; }
    }

    // @interface RSKTouchView : UIView
    [BaseType(typeof(UIView))]
    interface RSKTouchView
    {
        // @property (nonatomic, weak) UIView * receiver;
        [Export("receiver", ArgumentSemantic.Weak)]
        UIView Receiver { get; set; }
    }

    // @interface RSKImageCropper (UIApplication)
    [Category]
    [BaseType(typeof(UIApplication))]
    interface UIApplication_RSKImageCropper
    {
        // +(UIApplication *)rsk_sharedApplication;
        [Static]
        [Export("rsk_sharedApplication")]
        //[Verify(MethodToProperty)]
        UIApplication Rsk_sharedApplication { get; }
    }

    //// @interface RSKImageCropper (UIImage)
    //[Category]
    //[BaseType(typeof(UIImage))]
    //interface UIImage_RSKImageCropper
    //{
    //    // -(UIImage *)fixOrientation;
    //    [Export("fixOrientation")]
    //    //[Verify(MethodToProperty)]
    //    UIImage FixOrientation { get; }

    //    // -(UIImage *)rotateByAngle:(CGFloat)angleInRadians;
    //    [Export("rotateByAngle:")]
    //    UIImage RotateByAngle(nfloat angleInRadians);
    //}

}
