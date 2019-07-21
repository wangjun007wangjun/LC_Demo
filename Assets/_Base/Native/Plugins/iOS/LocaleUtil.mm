//
//  LocaleUtil.m
//  Unity-iPhone
//
//  Created by mhy on 2018/12/12.
//

#import <Foundation/Foundation.h>
#import "LocaleUtil.h"

@implementation LocaleUtil

+ (NSString *)getLanguage {
    return [[NSLocale currentLocale] objectForKey:NSLocaleLanguageCode];
}

+ (NSString *)getCountry {
    NSLocale *locale = [NSLocale currentLocale];
    return [[[locale localeIdentifier] componentsSeparatedByString:@"_"] objectAtIndex:1];
}

@end

extern char* cStringCopy(const char* string);

extern "C" {

    const char * getLanguage(){
        return cStringCopy([[LocaleUtil getLanguage] UTF8String]);
    }
    
    const char * getCountry() {
        return cStringCopy([[LocaleUtil getCountry] UTF8String]);
    }
}
