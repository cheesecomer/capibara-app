using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using RSKImageCropper;

namespace RSKImageCropper
{
    [StructLayout (LayoutKind.Sequential)]
    public struct RSKLineSegment
    {
        public CGPoint start;

        public CGPoint end;
    }

    static class CFunctions
    {
        // extern CGPoint RSKRectCenterPoint (CGRect rect);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern CGPoint RSKRectCenterPoint (CGRect rect);

        // extern CGRect RSKRectScaleAroundPoint (CGRect rect, CGPoint point, CGFloat sx, CGFloat sy);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern CGRect RSKRectScaleAroundPoint (CGRect rect, CGPoint point, nfloat sx, nfloat sy);

        // extern _Bool RSKPointIsNull (CGPoint point);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern bool RSKPointIsNull (CGPoint point);

        // extern CGPoint RSKPointRotateAroundPoint (CGPoint point, CGPoint pivot, CGFloat angle);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern CGPoint RSKPointRotateAroundPoint (CGPoint point, CGPoint pivot, nfloat angle);

        // extern CGFloat RSKPointDistance (CGPoint p1, CGPoint p2);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern nfloat RSKPointDistance (CGPoint p1, CGPoint p2);

        // extern RSKLineSegment RSKLineSegmentMake (CGPoint start, CGPoint end);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern RSKLineSegment RSKLineSegmentMake (CGPoint start, CGPoint end);

        // extern RSKLineSegment RSKLineSegmentRotateAroundPoint (RSKLineSegment lineSegment, CGPoint pivot, CGFloat angle);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern RSKLineSegment RSKLineSegmentRotateAroundPoint (RSKLineSegment lineSegment, CGPoint pivot, nfloat angle);

        // extern CGPoint RSKLineSegmentIntersection (RSKLineSegment ls1, RSKLineSegment ls2);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern CGPoint RSKLineSegmentIntersection (RSKLineSegment ls1, RSKLineSegment ls2);

        // extern NSString * RSKLocalizedString (NSString *key, NSString *comment);
        [DllImport ("__Internal")]
        //[Verify (PlatformInvoke)]
        static extern NSString RSKLocalizedString (NSString key, NSString comment);
    }

    //[Native]
    public enum RSKImageCropMode
    {
        Circle,
        Square,
        Custom
    }
}
