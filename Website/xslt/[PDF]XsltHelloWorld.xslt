<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library"
  xmlns:FergusonMoriyama.Pdf.XsltHelper="urn:FergusonMoriyama.Pdf.XsltHelper"
  xmlns:ibex="http://www.xmlpdf.com/2003/ibex/Format"
  exclude-result-prefixes="msxml umbraco.library FergusonMoriyama.Pdf.XsltHelper">

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:param name="currentPage"/>
  
  <xsl:template match="/">

    <!-- These two headers tell PDF for Umbraco to render a PDF -->
    <xsl:value-of select="FergusonMoriyama.Pdf.XsltHelper:SetResponseContentType('text/xsl')"/>
    <!-- If you remove this you'll see the FO output in the browser -->
    <xsl:value-of select="FergusonMoriyama.Pdf.XsltHelper:AppendResponseHeader('X-Pdf-Render', 'true')"/>
    
    <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">
      <fo:layout-master-set>
        <fo:simple-page-master master-name="A4" page-width="297mm" page-height="210mm" margin-top="1cm" margin-bottom="1cm" margin-left="1cm" margin-right="1cm">
          <fo:region-body margin="3cm"/>
          <fo:region-before extent="2cm"/>
          <fo:region-after extent="2cm"/>
          <fo:region-start extent="2cm"/>
          <fo:region-end extent="2cm"/>
        </fo:simple-page-master>
      </fo:layout-master-set>
  
      <fo:page-sequence master-reference="A4" format="A">
        <fo:flow flow-name="xsl-region-body">
          <fo:block>
            <fo:inline font-weight="bold">Hello world!</fo:inline>
          </fo:block>
        </fo:flow>
      </fo:page-sequence>
    </fo:root>
    
  </xsl:template>
    
</xsl:stylesheet>