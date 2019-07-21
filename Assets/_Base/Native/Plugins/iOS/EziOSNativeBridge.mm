//
//  Created by weiwei on 18/07/03.
//

#import "EziOSNativeBridge.h"
#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

#define SCREEN_WIDTH [UIScreen mainScreen].bounds.size.width
#define SCREEN_HEIGHT [UIScreen mainScreen].bounds.size.height

@implementation Device

+(NSString *)getUniqueID {
    NSString *key = [[NSBundle mainBundle]bundleIdentifier];
    NSString *strUUID = (NSString *)[KeyChainStore load:key];

    //首次执行该方法时，uuid为空
    if ([strUUID isEqualToString:@""] || !strUUID) {
        //生成一个uuid的方法
        CFUUIDRef uuidRef = CFUUIDCreate(kCFAllocatorDefault);
        strUUID = (NSString *)CFBridgingRelease(CFUUIDCreateString(kCFAllocatorDefault,uuidRef));
        //strUUID = [KeyChainStore appleIFA];
        //将该uuid保存到keychain
        [KeyChainStore save:key data:strUUID];
    }
    return strUUID;
}

+(float)getBatteryLevel {
    [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
    return [[UIDevice currentDevice] batteryLevel];
}

+(bool)checkMicrophone {
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeAudio];
    if (authStatus == AVAuthorizationStatusDenied || authStatus == AVAuthorizationStatusRestricted) {
        UIAlertController *alert = [UIAlertController alertControllerWithTitle:NSLocalizedString(@"nomic", @"") message:nil preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *okBtn = [UIAlertAction actionWithTitle:NSLocalizedString(@"iknow", @"") style:UIAlertActionStyleDefault handler:nil];
        UIAlertAction *go = [UIAlertAction actionWithTitle:NSLocalizedString(@"setting", @"") style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
            [[UIApplication sharedApplication]openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
        }];//cancel字体加粗

        [alert addAction:okBtn];
        [alert addAction:go];
        [[[[UIApplication sharedApplication]keyWindow]rootViewController] presentViewController:alert animated:YES completion:nil];
        return NO;
    }
    return YES;
}

@end


@implementation KeyChainStore

+ (void)save:(NSString *)service data:(id)data {
    //Get search dictionary
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    //Delete old item before add new item
    SecItemDelete((CFDictionaryRef)keychainQuery);
    //Add new object to search dictionary(Attention:the data format)
    [keychainQuery setObject:[NSKeyedArchiver archivedDataWithRootObject:data] forKey:(id)kSecValueData];
    //Add item to keychain with the search dictionary
    SecItemAdd((CFDictionaryRef)keychainQuery, NULL);
}

+ (id)load:(NSString *)service {
    id ret = nil;
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    //Configure the search setting
    //Since in our simple case we are expecting only a single attribute to be returned (the password) we can set the attribute kSecReturnData to kCFBooleanTrue
    [keychainQuery setObject:(id)kCFBooleanTrue forKey:(id)kSecReturnData];
    [keychainQuery setObject:(id)kSecMatchLimitOne forKey:(id)kSecMatchLimit];
    CFDataRef keyData = NULL;
    if (SecItemCopyMatching((CFDictionaryRef)keychainQuery, (CFTypeRef *)&keyData) == noErr) {
        @try {
            ret = [NSKeyedUnarchiver unarchiveObjectWithData:(__bridge NSData *)keyData];
        } @catch (NSException *e) {
            NSLog(@"Unarchive of %@ failed: %@", service, e);
        } @finally {
        }
    }
    if (keyData)
        CFRelease(keyData);
    return ret;
}

+ (void)deleteKeyData:(NSString *)service {
    NSMutableDictionary *keychainQuery = [self getKeychainQuery:service];
    SecItemDelete((CFDictionaryRef)keychainQuery);
}

//内部方法
+ (NSMutableDictionary *)getKeychainQuery:(NSString *)service {
    return [NSMutableDictionary dictionaryWithObjectsAndKeys:
            (id)kSecClassGenericPassword,(id)kSecClass,
            service, (id)kSecAttrService,
            service, (id)kSecAttrAccount,
            (id)kSecAttrAccessibleAfterFirstUnlock,(id)kSecAttrAccessible,
            nil];
}

//获取的广告id，用户可以轻易的重置
+ (NSString *)appleIFA {    //[[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString]
    NSString *ifa = nil;
    Class ASIdentifierManagerClass = NSClassFromString(@"ASIdentifierManager");
    if (ASIdentifierManagerClass) { // a dynamic way of checking if AdSupport.framework is available
        SEL sharedManagerSelector = NSSelectorFromString(@"sharedManager");
        id sharedManager = ((id (*)(id, SEL))[ASIdentifierManagerClass methodForSelector:sharedManagerSelector])(ASIdentifierManagerClass, sharedManagerSelector);
        SEL advertisingIdentifierSelector = NSSelectorFromString(@"advertisingIdentifier");
        NSUUID *advertisingIdentifier = ((NSUUID* (*)(id, SEL))[sharedManager methodForSelector:advertisingIdentifierSelector])(sharedManager, advertisingIdentifierSelector);
        ifa = [advertisingIdentifier UUIDString];
    }
    return ifa;
}

@end


@implementation Clipboard

+ (void)setText: (NSString *)text {
    [UIPasteboard generalPasteboard].string = text;
}

+ (NSString *)getText {
    return [UIPasteboard generalPasteboard].string;
}

@end


@implementation ShareNative

+ (void)shareImage:(NSString *)path text:(NSString *)text {
    UIImage *image = [UIImage imageWithContentsOfFile:path];
    NSArray *postItems = @[image];
    UIActivityViewController *avc = [[UIActivityViewController alloc]initWithActivityItems:postItems applicationActivities:nil];
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [avc respondsToSelector:@selector(popoverPresentationController)]) {
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:avc];
        [popup presentPopoverFromRect:CGRectMake(SCREEN_WIDTH/2, SCREEN_HEIGHT, 0, 0)
                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view
             permittedArrowDirections:UIPopoverArrowDirectionDown
                             animated:YES];
    } else {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:avc animated:YES completion:nil];
    }
}

//Method for text sharing
+ (void)shareText:(NSString *)text url:(NSString *)url {
    NSURL *nsURL = [NSURL URLWithString:url];
    NSArray *postItems  = @[text, nsURL];
    UIActivityViewController *avc = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad &&  [avc respondsToSelector:@selector(popoverPresentationController)]) {
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:avc];
        [popup presentPopoverFromRect:CGRectMake(SCREEN_WIDTH/2, SCREEN_HEIGHT, 0, 0)
                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view
             permittedArrowDirections:UIPopoverArrowDirectionDown
                             animated:YES];
    } else {
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:avc animated:YES completion:nil];
    }
}

@end

char* cstrcpy (NSString* nsstring)
{
    if (nsstring == NULL) {
        return NULL;
    }
    // convert from NSString to char with utf8 encoding
    const char* string = [nsstring cStringUsingEncoding:NSUTF8StringEncoding];
    if (string == NULL) {
        return NULL;
    }

    // create char copy with malloc and strcpy
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

char * cstrcpy(const char* str) {
    if (str == NULL)
        return NULL;

    char * res = (char *)malloc(strlen(str) + 1);
    strcpy(res, str);
    return res;
}


extern "C" {
    const char* GetSettingsURL () {
         NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
         return cstrcpy(url.absoluteString);
    }

    void OpenSettings () {
        NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
        [[UIApplication sharedApplication] openURL: url];
    }

    const char * getUniqueID() {
        return cstrcpy([[Device getUniqueID] UTF8String]);
    }

    float getBatteryLevel() {
        return [Device getBatteryLevel];
    }

    bool checkMicrophone() {
        return [Device checkMicrophone];
    }

    void setTextToClipboard(const char* text) {
        [Clipboard setText:[NSString stringWithUTF8String:text]];
    }

    const char * getTextFromClipboard() {
        return cstrcpy([[Clipboard getText] UTF8String]);
    }

    void shareText(const char * text, const char * url) {
        [ShareNative shareText:[NSString stringWithUTF8String:text] url:[NSString stringWithUTF8String:url]];
    }

    void shareImage(const char * path, const char * text) {
        [ShareNative shareImage:[NSString stringWithUTF8String:path] text:[NSString stringWithUTF8String:text]];
    }
}
