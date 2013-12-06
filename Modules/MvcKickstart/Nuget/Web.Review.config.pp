<?xml version="1.0"?>
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
	<appSettings>
		<add key="Metrics:Prefix" value="$rootnamespace$.Review." xdt:Locator="Match(key)" xdt:Transform="SetAttributes" />
	</appSettings>
	<cassette rewriteHtml="false" debug="false" xdt:Transform="Replace" />
	<system.web>
		<compilation xdt:Transform="RemoveAttributes(debug)" />
		<customErrors mode="RemoteOnly" xdt:Transform="SetAttributes" />
		<caching>
			<outputCache enableOutputCache="true" xdt:Transform="SetAttributes" />
		</caching>
	</system.web>
	<log4net>
		<appender name="SmtpAppender" type="MvcKickstart.Infrastructure.SmtpCachingAppender, MvcKickstart" xdt:Transform="Insert">
			<to value="notset@localhost" />
			<from value="do_not_reply@localhost.com" />
			<subject value="Warn :: $rootnamespace$ (Review)" />
			<smtpHost value="localhost" />
			<bufferSize value="20" />
			<lossy value="true" />
			<evaluator type="log4net.Core.LevelEvaluator">
				<threshold value="WARN" />
			</evaluator>
			<layout type="log4net.Layout.PatternLayout,log4net">
				<conversionPattern value="%property{log4net:HostName} :: %level :: %message %newlineLogger: %logger%newlineThread: %thread%newlineDate: %date%newlineNDC: %property{NDC}%newlineUrl: %property{CurrentRequestUrl}%newlineUser: %property{CurrentRequestUsername}%newlineReferrer: %property{CurrentRequestReferrer}%newlineUser-Agent:%property{CurrentRequestUserAgent}%newline%newline" />
			</layout>
		</appender>
		<root>
			<appender-ref ref="SmtpAppender" xdt:Transform="Insert" />
		</root>
	</log4net>
</configuration>