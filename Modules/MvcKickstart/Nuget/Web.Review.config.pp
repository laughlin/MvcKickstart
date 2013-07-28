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
		<httpHandlers>
			<add path="routes.axd" xdt:Locator="Match(path)" xdt:Transform="Remove" />
		</httpHandlers>
	</system.web>
	<system.webServer>
		<handlers>
			<add name="AttributeRouting" xdt:Locator="Match(name)" xdt:Transform="Remove" />
		</handlers>
	</system.webServer>
	<log4net>
		<appender name="SmtpAppender" type="log4net.Appender.SmtpAppender,log4net" xdt:Transform="Insert">
			<to value="notset@localhost" />
			<from value="do_not_reply@localhost.com" />
			<subject value="Error :: $rootnamespace$ (Review)" />
			<smtpHost value="localhost" />
			<bufferSize value="512" />
			<lossy value="false" />
			<filter type="log4net.Filter.LevelRangeFilter">
				<param name="LevelMin" value="ERROR" />
				<param name="LevelMax" value="FATAL" />
			</filter>
			<layout type="log4net.Layout.PatternLayout,log4net">
				<conversionPattern value="%property{log4net:HostName} :: %level :: %message %newlineLogger: %logger%newlineThread: %thread%newlineDate: %date%newlineNDC: %property{NDC}%newlineUrl: %property{CurrentRequestUrl}%newlineUser: %property{CurrentRequestUsername}%newlineReferrer: %property{CurrentRequestReferrer}%newlineUser-Agent:%property{CurrentRequestUserAgent}%newline%newline" />
			</layout>
		</appender>
		<appender name="SmtpWarnAppender" type="log4net.Appender.SmtpAppender,log4net" xdt:Transform="Insert">
			<to value="notset@localhost" />
			<from value="do_not_reply@localhost.com" />
			<subject value="Warn :: $rootnamespace$ (Review)" />
			<smtpHost value="localhost" />
			<bufferSize value="512" />
			<lossy value="false" />
			<filter type="log4net.Filter.LevelRangeFilter">
				<param name="LevelMin" value="WARN" />
				<param name="LevelMax" value="WARN" />
			</filter>
			<layout type="log4net.Layout.PatternLayout,log4net">
				<conversionPattern value="%property{log4net:HostName} :: %level :: %message %newlineLogger: %logger%newlineThread: %thread%newlineDate: %date%newlineNDC: %property{NDC}%newlineUrl: %property{CurrentRequestUrl}%newlineUser: %property{CurrentRequestUsername}%newlineReferrer: %property{CurrentRequestReferrer}%newlineUser-Agent:%property{CurrentRequestUserAgent}%newline%newline" />
			</layout>
		</appender>
		<root>
			<appender-ref ref="SmtpAppender" xdt:Transform="Insert" />
			<appender-ref ref="SmtpWarnAppender" xdt:Transform="Insert" />
		</root>
	</log4net>
</configuration>