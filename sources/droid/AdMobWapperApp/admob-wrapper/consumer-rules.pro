-keepattributes Signature,InnerClasses,EnclosingMethod

# The .NET binding invokes these Java names through JNI. Consumer minification
# must not rename or remove wrapper entry points or callback interfaces.
-keep class it.amdev.admob.wrapper.** { *; }
