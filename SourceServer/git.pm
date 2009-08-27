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
use warnings;
use Cwd;
use Cwd 'abs_path';
use constant FILE_LOOKUP_TABLE => 'FILE_LOOKUP_TABLE';
 
sub new {
    my $proto = shift;
    my $class = ref($proto) || $proto;
    my $self = {};
    bless($self, $class);
    
    $$self{FILE_LOOKUP_TABLE} = ();
    
    return($self);
}

sub GatherFileInformation {
    my $self = shift;
    my $sourcePath = shift;
	
	my $currentDirectory = getcwd();
	chdir($sourcePath);
	AddMatchingCandidatesToFileLookupTable($self);
	chdir($currentDirectory);
	
    return(keys %{$$self{FILE_LOOKUP_TABLE}} != 0);
}

sub GetRootDirectoryOfRepository {
	my $command_to_get_relative_repository_root = "git rev-parse --show-cdup";
	my $relative_root = ExecuteCommand($command_to_get_relative_repository_root);
	
	if ($relative_root eq "") {
		$relative_root = ".";
	}
	
	my $full_path_to_root = abs_path($relative_root) . "/";
	$full_path_to_root = StandardizePathsToBackslash($full_path_to_root);
	
	return($full_path_to_root);
}
sub StandardizePathsToBackslash {
	my $path = shift;
	$path =~ s/\//\\/g;
	return ($path);
}

sub AddMatchingCandidatesToFileLookupTable {
	my $self = shift;
	
	my $repositoryPath = GetRootDirectoryOfRepository();
	foreach my $currentLine (GetListOfCandidateFilesToBeIndexed()) {
		AddSourceIndexedStringToTable($self, $currentLine, $repositoryPath);
    }
}

sub GetListOfCandidateFilesToBeIndexed {
	my $treeId = GetIdForRepositoryTree();

	my $candidates = `"git --no-pager ls-tree -r --full-name $treeId"`;
	return(split(/\n/, $candidates))
}
sub AddSourceIndexedStringToTable {
	my $self = shift;
	my $lineToIndex = shift;
	my $repositoryPath = shift;
	
	if ($lineToIndex =~ m/^.*\s(.*)\t(.*)$/i) {
		my $objectId = $1;
		my $relativeFilePath = $2;
		my $localFile = StandardizeFilename($repositoryPath . $relativeFilePath);
		#print "Local File: $localFile\n";
		@{$$self{FILE_LOOKUP_TABLE}{$localFile}} = ( { }, "$localFile*$objectId" );
    }
}

sub GetIdForRepositoryTree {
	return(ExecuteCommand("git --no-pager log -1 --pretty=format:\%T")); # TODO: die if null
}
sub StandardizeFilename {
	my $fileName = shift;
	
	$fileName = StandardizePathsToBackslash($fileName);
	$fileName = lc($fileName);
	
	return($fileName);
}

sub GetFileInfo {
    my $self = shift;
    my $localFile = shift;
	
	$localFile = StandardizeFilename($localFile);
	
    if (defined $$self{FILE_LOOKUP_TABLE}{$localFile}) {
        return(@{$$self{FILE_LOOKUP_TABLE}{$localFile}});
    }
	
	return(undef);
}

sub LongName {
    return("Git");
}

sub SourceStreamVariables {
    my $self = shift;
    my @stream;
	
	my $repositoryId  = GetRepositoryId();
	my $originNode = GetOriginRepository();
	
	push(@stream, "GIT_REPO_ID=$repositoryId");
	push(@stream, "GIT_ORIGIN_NODE=$originNode");
	push(@stream, "GIT_EXTRACT_TARGET=%targ%\\%GIT_REPO_ID%\\%var2%\\%fnfile%(%var1%)");
    push(@stream, "GIT_EXTRACT_CMD=gitcontents.bat \"%GIT_ORIGIN_NODE%\" \"%targ%\\%GIT_REPO_ID%\\.localRepo\" %var2% \"%git_extract_target%\"");
    return (@stream);
}
sub GetRepositoryId {
	return(GetSha1OfFirstCommand());
}
sub GetSha1OfFirstCommand {
	return(ExecuteCommand("git rev-list --reverse master | head -1"));
}
sub GetOriginRepository {
	foreach my $repositoryToEvaluate(GetRemoteRepositories()) {
		if ($repositoryToEvaluate =~ m/^(origin)\t(.*)$/i) {
			return($2);
		}
	}
	
	return(undef) # TODO: die if null?
}
sub GetRemoteRepositories {
	my $remoteRepositories = `git --no-pager remote -v"`;
	return(split(/\r\n/, $remoteRepositories))
}

sub ExecuteCommand {
	my $commandToExecute = shift;
	
	my $commandOutput = `$commandToExecute`;
	
	if ($? < 0) {
		die "Execution of command \"$commandToExecute\" failed.";
	}
	
	$commandOutput = RemoveTrailingLineBreakCharacter($commandOutput);
	return($commandOutput)
}
sub RemoveTrailingLineBreakCharacter {
	my $valueToTrim = shift;
	$valueToTrim =~ s/[\r\n]+//g; #BUG: TOO GREEDY!
	return($valueToTrim);
}