using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using PhotoTweaks;
using UIKit;

namespace PhotoTweaks
{
    // @interface PhotoTweaksViewController : UIViewController
    [BaseType(typeof(UIViewController), Delegates = new string[] { "Delegate" }, Events = new Type[] { typeof(PhotoTweaksViewControllerDelegate) })]
    interface PhotoTweaksViewController
    {
        // @property (readonly, nonatomic, strong) UIImage * image;
        [Export("image", ArgumentSemantic.Strong)]
        UIImage Image { get; }

        // @property (assign, nonatomic) BOOL autoSaveToLibray;
        [Export("autoSaveToLibray")]
        bool AutoSaveToLibray { get; set; }

        // @property (assign, nonatomic) CGFloat maxRotationAngle;
        [Export("maxRotationAngle")]
        nfloat MaxRotationAngle { get; set; }

        [Wrap("WeakDelegate")]
        PhotoTweaksViewControllerDelegate Delegate { get; set; }

        // @property (nonatomic, weak) id<PhotoTweaksViewControllerDelegate> delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        // @property (nonatomic, strong) UIColor * saveButtonTitleColor;
        [Export("saveButtonTitleColor", ArgumentSemantic.Strong)]
        UIColor SaveButtonTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * saveButtonHighlightTitleColor;
        [Export("saveButtonHighlightTitleColor", ArgumentSemantic.Strong)]
        UIColor SaveButtonHighlightTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * cancelButtonTitleColor;
        [Export("cancelButtonTitleColor", ArgumentSemantic.Strong)]
        UIColor CancelButtonTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * cancelButtonHighlightTitleColor;
        [Export("cancelButtonHighlightTitleColor", ArgumentSemantic.Strong)]
        UIColor CancelButtonHighlightTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * resetButtonTitleColor;
        [Export("resetButtonTitleColor", ArgumentSemantic.Strong)]
        UIColor ResetButtonTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * resetButtonHighlightTitleColor;
        [Export("resetButtonHighlightTitleColor", ArgumentSemantic.Strong)]
        UIColor ResetButtonHighlightTitleColor { get; set; }

        // @property (nonatomic, strong) UIColor * sliderTintColor;
        [Export("sliderTintColor", ArgumentSemantic.Strong)]
        UIColor SliderTintColor { get; set; }

        // -(instancetype)initWithImage:(UIImage *)image;
        [Export("initWithImage:")]
        IntPtr Constructor(UIImage image);
    }

    // @protocol PhotoTweaksViewControllerDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface PhotoTweaksViewControllerDelegate
    {
        // @required -(void)photoTweaksController:(PhotoTweaksViewController *)controller didFinishWithCroppedImage:(UIImage *)croppedImage;
        [Abstract]
        [Export("photoTweaksController:didFinishWithCroppedImage:")]
        [EventArgs("PhotoTweaksViewControllerCropped")]
        [EventName("Cropped")]
        void DidFinish(PhotoTweaksViewController controller, UIImage croppedImage);

        // @required -(void)photoTweaksControllerDidCancel:(PhotoTweaksViewController *)controller;
        [Abstract]
        [Export("photoTweaksControllerDidCancel:")]
        [EventArgs("PhotoTweaksViewControllerCanceled")]
        [EventName("Canceled")]
        void DidCancel(PhotoTweaksViewController controller);
    }

    // @interface PhotoContentView : UIView
    [BaseType(typeof(UIView))]
    interface PhotoContentView
    {
        // @property (nonatomic, strong) UIImageView * imageView;
        [Export("imageView", ArgumentSemantic.Strong)]
        UIImageView ImageView { get; set; }

        // @property (nonatomic, strong) UIImage * image;
        [Export("image", ArgumentSemantic.Strong)]
        UIImage Image { get; set; }
    }

    // @protocol CropViewDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface CropViewDelegate
    {
        // @required -(void)cropEnded:(CropView *)cropView;
        [Abstract]
        [Export("cropEnded:")]
        void CropEnded(CropView cropView);

        // @required -(void)cropMoved:(CropView *)cropView;
        [Abstract]
        [Export("cropMoved:")]
        void CropMoved(CropView cropView);
    }

    // @interface CropView : UIView
    [BaseType(typeof(UIView))]
    interface CropView
    {
    }

    // @interface PhotoTweakView : UIView
    [BaseType(typeof(UIView))]
    interface PhotoTweakView
    {
        // @property (readonly, assign, nonatomic) CGFloat angle;
        [Export("angle")]
        nfloat Angle { get; }

        // @property (readonly, assign, nonatomic) CGPoint photoContentOffset;
        [Export("photoContentOffset", ArgumentSemantic.Assign)]
        CGPoint PhotoContentOffset { get; }

        // @property (readonly, nonatomic, strong) CropView * cropView;
        [Export("cropView", ArgumentSemantic.Strong)]
        CropView CropView { get; }

        // @property (readonly, nonatomic, strong) PhotoContentView * photoContentView;
        [Export("photoContentView", ArgumentSemantic.Strong)]
        PhotoContentView PhotoContentView { get; }

        // @property (readonly, nonatomic, strong) UISlider * slider;
        [Export("slider", ArgumentSemantic.Strong)]
        UISlider Slider { get; }

        // @property (readonly, nonatomic, strong) UIButton * resetBtn;
        [Export("resetBtn", ArgumentSemantic.Strong)]
        UIButton ResetBtn { get; }

        // -(instancetype)initWithFrame:(CGRect)frame image:(UIImage *)image maxRotationAngle:(CGFloat)maxRotationAngle;
        [Export("initWithFrame:image:maxRotationAngle:")]
        IntPtr Constructor(CGRect frame, UIImage image, nfloat maxRotationAngle);

        // -(instancetype)initWithFrame:(CGRect)frame image:(UIImage *)image;
        [Export("initWithFrame:image:")]
        IntPtr Constructor(CGRect frame, UIImage image);

        // -(CGPoint)photoTranslation;
        [Export("photoTranslation")]
        CGPoint PhotoTranslation { get; }
    }

    // @interface Tweak (UIColor)
    [Category]
    [BaseType(typeof(UIColor))]
    interface UIColor_Tweak
    {
        // +(UIColor *)cancelButtonColor;
        [Static]
        [Export("cancelButtonColor")]
        UIColor CancelButtonColor { get; }

        // +(UIColor *)cancelButtonHighlightedColor;
        [Static]
        [Export("cancelButtonHighlightedColor")]
        UIColor CancelButtonHighlightedColor { get; }

        // +(UIColor *)saveButtonColor;
        [Static]
        [Export("saveButtonColor")]
        UIColor SaveButtonColor { get; }

        // +(UIColor *)saveButtonHighlightedColor;
        [Static]
        [Export("saveButtonHighlightedColor")]
        UIColor SaveButtonHighlightedColor { get; }

        // +(UIColor *)resetButtonColor;
        [Static]
        [Export("resetButtonColor")]
        UIColor ResetButtonColor { get; }

        // +(UIColor *)resetButtonHighlightedColor;
        [Static]
        [Export("resetButtonHighlightedColor")]
        UIColor ResetButtonHighlightedColor { get; }

        // +(UIColor *)cropLineColor;
        [Static]
        [Export("cropLineColor")]
        UIColor CropLineColor { get; }

        // +(UIColor *)gridLineColor;
        [Static]
        [Export("gridLineColor")]
        UIColor GridLineColor { get; }

        // +(UIColor *)maskColor;
        [Static]
        [Export("maskColor")]
        UIColor MaskColor { get; }

        // +(UIColor *)photoTweakCanvasBackgroundColor;
        [Static]
        [Export("photoTweakCanvasBackgroundColor")]
        UIColor PhotoTweakCanvasBackgroundColor { get; }
    }

    //[Static]
    //partial interface Constants
    //{
    //    // extern const CGFloat kMaxRotationAngle;
    //    [Field("kMaxRotationAngle", "__Internal")]
    //    nfloat kMaxRotationAngle { get; }

    //    // extern double PhotoTweaksVersionNumber;
    //    [Field("PhotoTweaksVersionNumber", "__Internal")]
    //    double PhotoTweaksVersionNumber { get; }

    //    // extern const unsigned char [] PhotoTweaksVersionString;
    //    [Field("PhotoTweaksVersionString", "__Internal")]
    //    byte[] PhotoTweaksVersionString { get; }
    //}

}
