# ------------------------------------------------------------------------
# git.pm
#
# Source Server module to handle adding version control information to
# PDB symbol files for source code that is stored in a Git repository.
#
# Copyright (c) 2009 ImaginaryRealities Software Company
# http://www.imaginaryrealities.com/post/2009/05/03/Indexing-source-code-in-Git-with-Source-Server.aspx
#
# With modifications by Jonathan Oliver (http://jonathan-oliver.blogspot.com)
# ------------------------------------------------------------------------
package GIT;

require Exporter;
use strict;
use File::Basename;
use Cwd 'abs_path';

sub new {
    my $proto = shift;
    my $class = ref($proto) || $proto;
    my $self = {};
    bless($self, $class);
    
    $$self{'FILE_LOOKUP_TABLE'} = ();
    
    return($self);
}

sub GatherFileInformation {
    my $self = shift;
    my $sourcePath = shift;
    my $serverHashReference = shift;
	
	my $hProcess;
	
	# get SHA1 of initial repository commit to be used to uniquely identify the repository (for caching).
	if (!open($hProcess, "git rev-list --reverse master | head -1 |")) {
		::warn_message("Unable to query for initial commit in $sourcePath");
		return();
	}
	$$self{'REPOSITORY_ID'} = <$hProcess>;
	$$self{'REPOSITORY_ID'} =~ s/[\r\n]+//g; #remove the line break on the end
	close($hProcess);
	
	if (!open($hProcess, "git --no-pager remote -v |")) {
		::warn_message("Unable to determine remote repositories for $sourcePath");
		return();
	}
	
	$$self{'ORIGIN_REPOSITORY_NODE'} = "";
	my $currentRepository;
	while ($currentRepository = <$hProcess>) {
		if ($currentRepository =~ m/^(origin)\t(.*)$/i) {
			$$self{'ORIGIN_REPOSITORY_NODE'} = $2
		}
	}
	close($hProcess);
	
    if (!open($hProcess, "git --no-pager log -1 --pretty=format:\%T -- $sourcePath |")) {
        ::warn_message("Unable to get the log for $sourcePath");
        return();
    }
    
    my $treeId;
    $treeId = <$hProcess>;
    close($hProcess);

	if (!open($hProcess, "git rev-parse --show-cdup |")) {
		::warn_message("Unable to determine git root directory.");
		return();
	}
	
    my $repositoryPath;
	$repositoryPath = <$hProcess>;
	$repositoryPath =~ s/[\r\n]+//g; #remove the line break on the end
	if ("" eq $repositoryPath) {
		$repositoryPath = ".";
	}
	$repositoryPath = abs_path($repositoryPath) . "/";
	$repositoryPath =~ s/\//\\/g;
	
	close($hProcess);
    
    if (!open($hProcess, "git --no-pager ls-tree -r --full-name $treeId $sourcePath |")) {
        ::warn_message("Unable to get the tree $treeId for $sourcePath");
        return();
    }

    my $currentLine;
    while ($currentLine = <$hProcess>) {
        if ($currentLine =~ m/^(.*)\s(.*)\s(.*)\t(.*)$/i) {
            my $mode;
            my $type;
            my $objectId;
            my $relativePath;
            
            $objectId = $3;

            $_ = $4;
            s/\//\\/g;
            $relativePath = $_;
            
            my $localFile;
            $localFile = $repositoryPath . $relativePath;
            @{$$self{'FILE_LOOKUP_TABLE'}{lc $localFile}} = ( { }, "$localFile*$objectId" );
        }
    }
    
    close($hProcess);
    
    return(keys %{$$self{'FILE_LOOKUP_TABLE'}} != 0);
}

sub GetFileInfo {
    my $self = shift;
    my $localFile = shift;
	
    if (defined $$self{'FILE_LOOKUP_TABLE'}{lc $localFile}) {
        return(@{$$self{'FILE_LOOKUP_TABLE'}{lc $localFile}});
    } else {
        return(undef);
    }
}

sub LongName {
    return("Git");
}

sub SourceStreamVariables {
    my $self = shift;
    my @stream;
	
	push(@stream, "GIT_REPO_ID=$$self{'REPOSITORY_ID'}");
	push(@stream, "GIT_ORIGIN_NODE=$$self{'ORIGIN_REPOSITORY_NODE'}");
	push(@stream, "GIT_EXTRACT_TARGET=%targ%\\%GIT_REPO_ID%\\%var2%\\%fnfile%(%var1%)");
    push(@stream, "GIT_EXTRACT_CMD=gitcontents.bat \"%GIT_ORIGIN_NODE%\" \"%targ%\\%GIT_REPO_ID%\\.localRepo\" %var2% \"%git_extract_target%\"");
    return (@stream);
}