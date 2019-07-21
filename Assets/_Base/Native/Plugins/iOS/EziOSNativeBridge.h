//
//  Created by weiwei on 18/7/03.
//

#import <Foundation/Foundation.h>

@interface Device : NSObject

+ (NSString *)getUniqueID;
+(float)getBatteryLevel;
+(bool)checkMicrophone;

@end

@interface KeyChainStore : NSObject

+ (void)save : (NSString *)service data : (id)data;
+(id)load:(NSString *)service;
+(void)deleteKeyData:(NSString *)service;
+(NSString *)appleIFA;

@end

@interface Clipboard : NSObject

+ (void)setText : (NSString *)text;
+(NSString *)getText;

@end

@interface ShareNative : NSObject

+ (void)shareImage : (NSString *)path text : (NSString *)text;
+(void)shareText:(NSString *)text url : (NSString *)url;

@end
