<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [ <!ENTITY nbsp "&#x00A0;"> ]>
<xsl:stylesheet 
				version="1.0" 
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
				xmlns:msxml="urn:schemas-microsoft-com:xslt"
				xmlns:umbraco.library="urn:umbraco.library" xmlns:Exslt.ExsltCommon="urn:Exslt.ExsltCommon" xmlns:Exslt.ExsltDatesAndTimes="urn:Exslt.ExsltDatesAndTimes" xmlns:Exslt.ExsltMath="urn:Exslt.ExsltMath" xmlns:Exslt.ExsltRegularExpressions="urn:Exslt.ExsltRegularExpressions" xmlns:Exslt.ExsltStrings="urn:Exslt.ExsltStrings" xmlns:Exslt.ExsltSets="urn:Exslt.ExsltSets" xmlns:Examine="urn:Examine" xmlns:TC="urn:TC" 
				exclude-result-prefixes="msxml umbraco.library Exslt.ExsltCommon Exslt.ExsltDatesAndTimes Exslt.ExsltMath Exslt.ExsltRegularExpressions Exslt.ExsltStrings Exslt.ExsltSets Examine TC ">

	<xsl:output method="html" omit-xml-declaration="yes"/>	
	<xsl:param name="currentPage"/>	
	<xsl:template match="/">

		<xsl:choose>
			<xsl:when test="$currentPage/wwsidetitle!=''">
				<xsl:value-of select="$currentPage/wwsidetitle"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$currentPage/@nodeName"/>
			</xsl:otherwise>
		</xsl:choose>

	</xsl:template>	
</xsl:stylesheet>