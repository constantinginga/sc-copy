<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxml="urn:schemas-microsoft-com:xslt"
  xmlns:umbraco.library="urn:umbraco.library"
  xmlns:FergusonMoriyama.Pdf.XsltHelper="urn:FergusonMoriyama.Pdf.XsltHelper"
  xmlns:fo="http://www.w3.org/1999/XSL/Format"
  xmlns:ibex="http://www.xmlpdf.com/2003/ibex/Format"
  exclude-result-prefixes="msxml umbraco.library FergusonMoriyama.Pdf.XsltHelper">

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:param name="currentPage"/>
  <xsl:variable name="webRootPath" select="FergusonMoriyama.Pdf.XsltHelper:GetWebRootPath()"/>

  <xsl:template match="/">

    <!-- Need physical path for including images -->
    
    <!-- These two headers tell PDF for Umbraco to render a PDF -->
    <xsl:value-of select="FergusonMoriyama.Pdf.XsltHelper:SetResponseContentType('text/xsl')"/>
    <!-- If you remove this you'll see the FO output in the browser -->
    <xsl:value-of select="FergusonMoriyama.Pdf.XsltHelper:AppendResponseHeader('X-Pdf-Render', 'true')"/>

    <!-- Uncomment below to force the browser to download the PDF -->
    <!-- <xsl:value-of select="FergusonMoriyama.Pdf.XsltHelper:AppendResponseHeader('X-Pdf-Force-Download', 'hello.pdf')"/> -->

    <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">

      <!-- Sets standard PDF Metadata -->
      <ibex:properties
        title="{$currentPage/@nodeName}"
        author="{$currentPage/@writerName}"
        subject=""
        keywords="metat,bacon,sheep"
        creator="PDF Creator for Umbraco" />

      <!-- Uncomment below to add protection to the PDF - optionally specify a password -->
      <!-- <ibex:security deny-print="true" deny-extract="true" deny-modify="true" user-password="" owner-password=""/> -->

      <!-- This defines a simple page layout with a heder and a footer -->
      <!-- See http://www.w3schools.com/xslfo/obj_layout-master-set.asp -->
      <fo:layout-master-set>
        <fo:simple-page-master master-name="master" page-width="210mm" page-height="297mm" margin-top="1cm" margin-bottom="1cm" margin-left="1cm" margin-right="1cm">
          <fo:region-body margin-top="1.5cm" margin-bottom="1.5cm" column-count="2" column-gap="0.5cm"/>
          <fo:region-before region-name="header" extent="3cm"/>
          <fo:region-after region-name="footer" extent="1.5cm"/>
        </fo:simple-page-master>
      </fo:layout-master-set>

      <!-- Main content starts within page sequence -->
      <fo:page-sequence master-reference="master">
        
        <!-- Doucment header -->
        <fo:flow flow-name="header">
          <fo:block>
            <fo:inline font-family="Arial" font-size="23pt" color="#3399ff">
              <xsl:value-of select="$currentPage/@nodeName"/>
            </fo:inline>
          </fo:block>
        </fo:flow>
        
        <!-- Doucment footer -->
        <fo:static-content flow-name="footer">
          <fo:block font-size="8pt" color="#aaaaaa">
            <fo:block text-align-last="justify">
              <xsl:value-of select="$currentPage/@nodeName"/> by 
              <xsl:value-of select="$currentPage/@writerName"/> - <xsl:value-of select="umbraco.library:CurrentDate()"/>
              
              <fo:leader leader-pattern="space"/>
              Page <fo:page-number/> of <fo:page-number-citation ref-id="last-page"/>
            </fo:block>
          </fo:block>
        </fo:static-content>
        
        <!-- Document Body -->
        <fo:flow flow-name="xsl-region-body">
          <fo:block>
            
            <!-- Turn the RTE into XML so it can be parsed -->
            <xsl:variable name="xhtml" select="FergusonMoriyama.Pdf.XsltHelper:RichTextToXml($currentPage/bodyText)"/>
            <xsl:apply-templates select="$xhtml"/>
            
          </fo:block>
          
          <!-- Having this before the closing tag of the body flow allows us to have a pager in the footer -->
          <fo:block id="last-page" keep-together.within-page="auto"></fo:block>
          
        </fo:flow>

      </fo:page-sequence>
    </fo:root>

  </xsl:template>

  <!-- The templates below match elements in your rich text areas and convert them to FO -->  
    
  <xsl:template match="a">
    <fo:basic-link color="blue" text-decoration="underline" external-destination="url('{@href}')">
      <xsl:apply-templates/>
    </fo:basic-link>
  </xsl:template>

  <xsl:template match="em">

    <fo:inline font-style="italic">
      <xsl:apply-templates/>
    </fo:inline>
  </xsl:template>

  <xsl:template match="strong">
    <fo:inline font-weight="bold">
      <xsl:apply-templates/>
    </fo:inline>
  </xsl:template>

  <xsl:template match="p">
    <fo:block margin-bottom="0.5cm">
      <xsl:apply-templates/>
    </fo:block>
  </xsl:template>

  <xsl:template match="li">
    <fo:list-item>
      <fo:list-item-label>
        <fo:block>-</fo:block>
      </fo:list-item-label>
      <fo:list-item-body>
        <fo:block margin-left="0.5cm">
          <xsl:apply-templates/>
        </fo:block>
      </fo:list-item-body>
    </fo:list-item>
  </xsl:template>

  <xsl:template match="ol">
    <fo:list-block>
      <xsl:apply-templates/>
    </fo:list-block>
  </xsl:template>

  <xsl:template match="img">
    <fo:block>
      <fo:external-graphic src="{$webRootPath}/{@src}" content-width="9cm"/>
    </fo:block>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="u">
    <fo:inline text-decoration="underline">
      <xsl:apply-templates/>
    </fo:inline>
  </xsl:template>

</xsl:stylesheet>