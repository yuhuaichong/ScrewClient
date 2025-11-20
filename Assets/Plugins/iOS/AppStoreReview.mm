//
//  AppStoreReview.mm
//  Unity-iOS bridge for App Store Review (系统原生评价弹窗)
//

#import <StoreKit/StoreKit.h>
#import <UIKit/UIKit.h>

extern "C" {
    
    // 调用系统原生评价弹窗（SKStoreReviewController）
    // iOS 10.3+ 支持
    // 注意：
    // - 一年内同一应用最多显示 3 次（系统控制）
    // - 系统决定是否显示（无法强制显示）
    // - 如果系统不显示，不会打开任何界面
    void _RequestInAppReview()
    {
        if (@available(iOS 10.3, *)) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [SKStoreReviewController requestReview];
                NSLog(@"[AppStoreReview] Requested in-app review (system will decide whether to show)");
            });
        } else {
            NSLog(@"[AppStoreReview] SKStoreReviewController requires iOS 10.3+");
        }
    }
}

